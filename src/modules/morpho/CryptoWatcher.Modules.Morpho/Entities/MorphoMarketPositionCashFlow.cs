using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Morpho.Entities;

public class MorphoMarketPositionCashFlow : ITokenCashFlow
{
    public Guid Id { get; set; }
    
    public DateTime Date { get; private set; }

    public CashFlowEvent Event { get; private set; } = null!;

    public CryptoTokenStatistic Token0 { get; private set; } = null!;
}