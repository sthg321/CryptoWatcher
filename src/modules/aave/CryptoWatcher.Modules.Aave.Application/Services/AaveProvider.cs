using CryptoWatcher.Extensions;
using CryptoWatcher.Modules.Aave.Application.Abstractions;
using CryptoWatcher.Modules.Aave.Application.Models;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.Modules.Aave.Models;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Aave.Application.Services;

public class AaveProvider : IAaveProvider
{
    private readonly IAaveGateway _aaveGateway;
    private readonly IAaveHealthFactorCalculator _aaveHealthFactorCalculator;

    public AaveProvider(
        IAaveGateway aaveApiClient,
        IAaveHealthFactorCalculator aaveHealthFactorCalculator)
    {
        _aaveGateway = aaveApiClient;
        _aaveHealthFactorCalculator = aaveHealthFactorCalculator;
    }

    public async Task<AavePositionsResponse> GetLendingPositionAsync(
        AaveProtocolConfiguration protocol,
        Wallet wallet)
    {
        var userReserves = await _aaveGateway.GetUserReservesDataAsync(protocol, wallet.Address);

        var reserveOutput = await _aaveGateway.GetMarketReservesDataAsync(protocol);

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
                throw new InvalidOperationException(
                    $"Reserve data not found for asset {userReserveData.UnderlyingAsset}");
            }

            var tokenDecimals = (byte)reserveData.Decimals;

            var priceDecimals =
                reserveOutput.NetworkBaseTokenPriceDecimals;

            var tokenPriceInUsd = reserveData.PriceInMarketReferenceCurrency.ToDecimal(priceDecimals);

            if (userReserveData.ScaledATokenBalance > 0)
            {
                var supplyPosition =
                    AaveLendingPositionFactory.CreateSupply(userReserveData, reserveData, tokenPriceInUsd,
                        tokenDecimals);

                result.Add(supplyPosition);
            }

            if (userReserveData.ScaledVariableDebt > 0)
            {
                var supplyPosition =
                    AaveLendingPositionFactory.CreateBorrow(userReserveData, reserveData, tokenPriceInUsd,
                        tokenDecimals);

                result.Add(supplyPosition);
            }
        }

        var healthFactor = _aaveHealthFactorCalculator.CalculateHealthFactor(result);

        return new AavePositionsResponse(result, healthFactor);
    }
}