using CryptoWatcher.Modules.Aave.Application.Abstractions;
using CryptoWatcher.Modules.Aave.Application.Models;
using CryptoWatcher.Modules.Aave.Entities;

namespace CryptoWatcher.Modules.Aave.Application.Services;

public class AaveHealthFactorCalculator : IAaveHealthFactorCalculator
{
    public double CalculateHealthFactor(IReadOnlyCollection<AavePosition> userPositions)
    {
        var collateral = 0m;
        var debt = 0m;

        foreach (var lendingPosition in userPositions)
        {
            foreach (var positionSnapshot in lendingPosition.Snapshots)
            {
                switch (lendingPosition.PositionType)
                {
                    case AavePositionType.Supplied:
                    {
                        collateral += positionSnapshot.Token0.AmountInUsd * (decimal)positionSnapshot.LiquidationLtv!;
                        break;
                    }
                    case AavePositionType.Borrowed:
                    {
                        debt +=  positionSnapshot.Token0.AmountInUsd;
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(userPositions));
                }
            }
        }

        if (debt == 0)
        {
            return double.MaxValue;
        }

        return (double)(collateral / debt);
    }
    
    public double CalculateHealthFactor(IReadOnlyCollection<AaveLendingPosition> userPositions)
    {
        var collateral = 0m;
        var debt = 0m;

        foreach (var lendingPosition in userPositions)
        {
            switch (lendingPosition.PositionType)
            {
                case AavePositionType.Supplied when (lendingPosition.IsCollateral ?? false):
                    collateral += lendingPosition.AmountInUsdUsd * lendingPosition.LiquidationLtv.GetValueOrDefault();
                    break;
                case AavePositionType.Borrowed:
                    debt += lendingPosition.AmountInUsdUsd;
                    break;
            }
        }

        if (debt == 0)
        {
            return double.MaxValue;
        }

        return (double)(collateral / debt);
    }
}