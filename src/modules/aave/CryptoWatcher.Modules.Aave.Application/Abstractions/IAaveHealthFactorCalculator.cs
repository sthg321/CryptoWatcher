using CryptoWatcher.Modules.Aave.Application.Models;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.Modules.Aave.Models;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Aave.Application.Abstractions;

public interface IAaveHealthFactorCalculator
{
    double CalculateHealthFactor(IReadOnlyCollection<AavePosition> userPositions);
    
    double CalculateHealthFactor(IReadOnlyCollection<AaveLendingPosition> userPositions,
        Dictionary<EvmAddress, AggregatedMarketReserveData> marketReserveOutput);
}