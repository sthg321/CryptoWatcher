using CryptoWatcher.Extensions;
using CryptoWatcher.Modules.Aave.Application.Abstractions;
using CryptoWatcher.Modules.Aave.Application.Abstractions.Client;
using CryptoWatcher.Modules.Aave.Application.Models;
using CryptoWatcher.Modules.Aave.Application.Services;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.Modules.Aave.Models;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Aave.Infrastructure.Services;

internal class AaveProvider : IAaveProvider
{
    private readonly IAaveApiClient _aaveApiClient;
    private readonly IAaveHealthFactorCalculator _aaveHealthFactorCalculator;

    public AaveProvider(IAaveApiClient aaveApiClient, IAaveHealthFactorCalculator aaveHealthFactorCalculator)
    {
        _aaveApiClient = aaveApiClient;
        _aaveHealthFactorCalculator = aaveHealthFactorCalculator;
    }

    public async Task<AavePositionsResponse> GetLendingPositionAsync(AaveChainConfiguration chain, Wallet wallet)
    {
        var userReserves =
            await _aaveApiClient.UiPoolDataProviderFetcher.GetUserReservesDataAsync(chain, wallet.Address);

        var reserveOutput =
            await _aaveApiClient.UiPoolDataProviderFetcher.GetMarketReservesDataAsync(chain);

        var marketData = reserveOutput.AggregatedMarketReserveData.ToDictionary(data => data.UnderlyingAsset);

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

            if (!marketData.TryGetValue(userReserveData.UnderlyingAsset, out var reserveData))
            {
                throw new Exception("Can't find reserve data");
            }

            var decimals = reserveOutput.NetworkBaseTokenPriceDecimals;

            if (userReserveData.ScaledATokenBalance > 0)
            {
                var suppliedPosition = new SuppliedAaveLendingPosition
                {
                    ScaleAmount = userReserveData.ScaledATokenBalance,
                    TokenAddress = EvmAddress.Create(userReserveData.UnderlyingAsset),
                    LiquidityIndex = reserveData.LiquidityIndex,
                    TokenPriceInUsd = reserveData.PriceInMarketReferenceCurrency.ToDecimal(decimals),
                    TokenDecimals = (byte)reserveData.Decimals,
                    LiquidationLtv = (decimal)Math.Round((double)reserveData.LiquidationLtv / 10000, 4),
                    IsCollateral = userReserveData.IsCollateral
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

        var healthFactor = _aaveHealthFactorCalculator.CalculateHealthFactor(
            result.OfType<CalculatableAaveLendingPosition>().ToArray(), marketData);

        return new AavePositionsResponse(result, healthFactor);
    }
}