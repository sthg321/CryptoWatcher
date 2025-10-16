using AaveClient;
using CryptoWatcher.Modules.Aave.Abstractions;
using CryptoWatcher.Modules.Aave.Models;
using CryptoWatcher.Extensions;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.ValueObjects;

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

        var userReserves =
            await _aaveApiClient.UiPoolDataProviderFetcher.GetUserReservesDataAsync(mainnet, networkInfo,
                wallet.Address);

        var reserveOutput = await _aaveApiClient.UiPoolDataProviderFetcher.GetReservesDataAsync(mainnet, networkInfo);
        var reserveDataDictionary = reserveOutput.ReservesData.ToDictionary(data => data.UnderlyingAsset);

        var result = new List<AaveLendingPosition>();

        foreach (var userReserveData in userReserves)
        {
            if (userReserveData.ScaledATokenBalance == 0 && userReserveData.ScaledVariableDebt == 0)
            {
                result.Add(new EmptyAaveLendingPosition
                {
                    TokenAddress = EvmAddress.Create(userReserveData.UnderlyingAsset)
                });

                continue;
            }

            if (!reserveDataDictionary.TryGetValue(userReserveData.UnderlyingAsset, out var reserveData))
            {
                throw new Exception("Can't find reserve data");
            }

            var decimals = reserveOutput.BaseCurrencyInfo.NetworkBaseTokenPriceDecimals;

            if (userReserveData.ScaledATokenBalance > 0)
            {
                var suppliedPosition = new SuppliedAaveLendingPosition
                {
                    ScaleAmount = userReserveData.ScaledATokenBalance,
                    TokenAddress = EvmAddress.Create(userReserveData.UnderlyingAsset),
                    LiquidityIndex = reserveData.LiquidityIndex,
                    TokenPriceInUsd = reserveData.PriceInMarketReferenceCurrency.ToDecimal(decimals),
                    TokenDecimals = (byte)reserveData.Decimals
                };

                result.Add(suppliedPosition);
            }

            if (userReserveData.ScaledVariableDebt > 0)
            {
                var borrowedPosition = new BorrowedAaveLendingPosition
                {
                    ScaleAmount = userReserveData.ScaledVariableDebt,
                    TokenAddress = EvmAddress.Create(userReserveData.UnderlyingAsset),
                    VariableBorrowIndex = reserveData.VariableBorrowIndex,
                    TokenPriceInUsd = reserveData.PriceInMarketReferenceCurrency.ToDecimal(decimals),
                    TokenDecimals = (byte)reserveData.Decimals
                };

                result.Add(borrowedPosition);
            }
        }

        return result;
    }

    private static AaveRegistry.SmartContractAddresses GetNetworkInfo(AaveNetwork aaveNetwork)
    {
        if (!Enum.TryParse<AaveNetworkType>(aaveNetwork.Name, out var network))
        {
            throw new ArgumentException(
                $"Network {aaveNetwork.Name} is not supported. Supported networks: {string.Join(", ", Enum.GetNames<AaveNetworkType>())}");
        }

        return AaveRegistry.NetworkToRpcUrl[network];
    }
}