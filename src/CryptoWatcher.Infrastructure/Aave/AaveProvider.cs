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

    public async Task<List<AaveLendingPosition>> GetLendingPositionAsync(AaveNetwork aaveNetwork, Wallet wallet,
        CancellationToken ct = default)
    {
        var mainnet = _aaveMainnetProvider.GetMainnetAddressByNetworkName(aaveNetwork);

        var networkInfo = GetNetworkInfo(aaveNetwork);

        var positions =
            await _aaveApiClient.UiPoolDataProviderFetcher.GetUserReservesDataAsync(mainnet, networkInfo,
                wallet.Address);

        var result = new List<AaveLendingPosition>();

        foreach (var userReserveData in positions)
        {
            if (userReserveData.ScaledATokenBalance == 0 && userReserveData.ScaledVariableDebt == 0)
            {
                var emptyPosition = new AaveLendingPosition
                {
                    TokenAddress = userReserveData.UnderlyingAsset,
                    Network = aaveNetwork,
                    Amount = 0,
                    PositionType = null
                };

                result.Add(emptyPosition);
                
                continue;
            }
            
            if (userReserveData.ScaledATokenBalance > 0)
            {
                var suppliedPosition = new AaveLendingPosition
                {
                    Amount = userReserveData.ScaledATokenBalance,
                    TokenAddress = userReserveData.UnderlyingAsset,
                    PositionType = AavePositionType.Supplied,
                    Network = aaveNetwork,
                };

                result.Add(suppliedPosition);
            }

            if (userReserveData.ScaledVariableDebt > 0)
            {
                var borrowedPosition = new AaveLendingPosition
                {
                    Amount = userReserveData.ScaledVariableDebt,
                    TokenAddress = userReserveData.UnderlyingAsset,
                    PositionType = AavePositionType.Borrowed,
                    Network = aaveNetwork,
                };

                result.Add(borrowedPosition);
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