using CryptoWatcher.Exceptions;
using CryptoWatcher.Modules.Aave.Application.Abstractions;
using CryptoWatcher.Modules.Aave.Application.Models;
using CryptoWatcher.Modules.Aave.Models;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Aave.Application.Services;

public class AaveHealthFactorCalculator : IAaveHealthFactorCalculator
{
    internal const string DebtGreatestThatCollateralExceptionMessage = "Debt can't be greatest that collateral";

    public double CalculateHealthFactor(IReadOnlyCollection<AaveLendingPosition> userPositions,
        Dictionary<EvmAddress, AggregatedMarketReserveData> marketReserveOutput)
    {
        var collateral = 0m;
        var debt = 0m;

        foreach (var lendingPosition in userPositions.Where(reserve => reserve is not EmptyAaveLendingPosition))
        {
            var marketData = marketReserveOutput[lendingPosition.TokenAddress];

            switch (lendingPosition)
            {
                case SuppliedAaveLendingPosition { IsCollateral: true } suppliedPosition:
                {
                    var liquidationThresholdFraction = marketData.ReserveLiquidationThreshold / 10000m;
                    collateral += suppliedPosition.CalculatePositionScaleInToken() * suppliedPosition.TokenPriceInUsd *
                                  liquidationThresholdFraction;
                    break;
                }
                case BorrowedAaveLendingPosition borrowedPosition:
                    debt += borrowedPosition.CalculatePositionScaleInToken() * borrowedPosition.TokenPriceInUsd;
                    break;
            }
        }

        if (debt > collateral)
        {
            throw new DomainException(DebtGreatestThatCollateralExceptionMessage);
        }

        if (debt == 0)
        {
            return int.MaxValue;
        }

        return (double)(collateral / debt);
    }
}