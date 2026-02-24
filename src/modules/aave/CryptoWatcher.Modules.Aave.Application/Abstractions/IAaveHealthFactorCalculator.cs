using CryptoWatcher.Modules.Aave.Application.Models;
using CryptoWatcher.Modules.Aave.Entities;

namespace CryptoWatcher.Modules.Aave.Application.Abstractions;

public interface IAaveHealthFactorCalculator
{
    double CalculateHealthFactor(IReadOnlyCollection<AavePosition> userPositions);
    
    double CalculateHealthFactor(IReadOnlyCollection<AaveLendingPosition> userPositions);
}