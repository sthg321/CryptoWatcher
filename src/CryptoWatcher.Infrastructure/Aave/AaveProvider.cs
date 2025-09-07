using System.Numerics;
using AaveClient;
using CryptoWatcher.AaveModule.Abstractions;
using CryptoWatcher.AaveModule.Entities;
using CryptoWatcher.AaveModule.Models;
using CryptoWatcher.Shared.Entities;
using AaveNetwork = CryptoWatcher.AaveModule.Models.AaveNetwork;

namespace CryptoWatcher.Infrastructure.Aave;

internal class AaveProvider : IAaveProvider
{
    private readonly IAaveApiClient _aaveApiClient;
    private readonly IAaveMainnetProvider _aaveMainnetProvider;

    public AaveProvider(IAaveApiClient aaveApiClient, IAaveMainnetProvider aaveMainnetProvider)
    {
        _aaveApiClient = aaveApiClient;
        _aaveMainnetProvider = aaveMainnetProvider;
    }

    public async Task<BigInteger> GetAssetPriceAsync(AaveNetwork aaveNetwork, string assetAddress)
    {
        var mainnet = _aaveMainnetProvider.GetMainnetAddressByNetworkName(aaveNetwork);

        var networkInfo = GetNetworkInfo(aaveNetwork);

        return await _aaveApiClient.OracleFetcher.GetAssetPriceAsync(mainnet, networkInfo.OracleAddress, assetAddress);
    }

    public async Task<AaveReserveData> GetLiquidityIndex(AaveNetwork aaveNetwork, string assetAddress)
    {
        var mainnet = _aaveMainnetProvider.GetMainnetAddressByNetworkName(aaveNetwork);

        var networkInfo = GetNetworkInfo(aaveNetwork);

        var reserveData =
            await _aaveApiClient.PoolFetcher.GetReserveDataAsync(mainnet, networkInfo.PoolAddress, assetAddress);

        // Преобразуем полученные данные в доменную модель
        return new AaveReserveData
        {
            Network = aaveNetwork,
            AssetAddress = assetAddress,
            LiquidityIndex = reserveData.LiquidityIndex,
            VariableBorrowIndex = reserveData.VariableBorrowIndex,
        };
    }

    public async Task<List<AaveLendingPosition>> GetLendingPositionAsync(AaveNetwork aaveNetwork, Wallet wallet,
        CancellationToken ct = default)
    {
        var mainnet = _aaveMainnetProvider.GetMainnetAddressByNetworkName(aaveNetwork);

        var networkInfo = GetNetworkInfo(aaveNetwork);

        var positions =
            await _aaveApiClient.UiPoolDataProviderFetcher.GetUserReservesDataAsync(mainnet, networkInfo,
                wallet.Address);

        var result = new List<AaveLendingPosition>();
        
        foreach (var positionsChunk in positions.Chunk(3))
        {
            var tasksToGetLiquidityIndex = positionsChunk.Select(position => GetLiquidityIndex(aaveNetwork, position.UnderlyingAsset));
            var liquidityIndexes = await Task.WhenAll(tasksToGetLiquidityIndex);

            foreach (var userReserveData in positionsChunk)    
            {
                if (userReserveData.ScaledATokenBalance == 0 && userReserveData.ScaledVariableDebt == 0)
                {
                    result.Add(AaveLendingPosition.CreateEmpty(aaveNetwork, userReserveData.UnderlyingAsset));

                    continue;
                }

                var reserveData = liquidityIndexes.First(index => index.AssetAddress == userReserveData.UnderlyingAsset);

                if (userReserveData.ScaledATokenBalance > 0)
                {
                    var suppliedPosition = new AaveLendingPosition
                    {
                        ScaleAmount = userReserveData.ScaledATokenBalance,
                        TokenAddress = userReserveData.UnderlyingAsset,
                        PositionType = AavePositionType.Supplied,
                        PoolIndex = reserveData.LiquidityIndex,
                        Network = aaveNetwork,
                    };

                    result.Add(suppliedPosition);
                }

                if (userReserveData.ScaledVariableDebt > 0)
                {
                    var borrowedPosition = new AaveLendingPosition
                    {
                        ScaleAmount = userReserveData.ScaledVariableDebt,
                        TokenAddress = userReserveData.UnderlyingAsset,
                        PositionType = AavePositionType.Borrowed,
                        PoolIndex = reserveData.VariableBorrowIndex,
                        Network = aaveNetwork,
                    };

                    result.Add(borrowedPosition);
                }
            }
        }
        
        return result;
    }

    private NetworkRegistry.NetworkInfo GetNetworkInfo(AaveNetwork aaveNetwork)
    {
        if (!Enum.TryParse<AaveNetworkType>(aaveNetwork.Name, out var network))
        {
            throw new ArgumentException(
                $"Network {aaveNetwork.Name} is not supported. Supported networks: {string.Join(", ", Enum.GetNames<AaveNetworkType>())}");
        }

        return NetworkRegistry.NetworkToRpcUrl[network];
    }
}