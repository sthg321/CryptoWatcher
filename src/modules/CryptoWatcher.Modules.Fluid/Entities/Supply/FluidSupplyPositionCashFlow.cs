using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Fluid.Entities.Supply;

public class FluidSupplyPositionCashFlow
{
    public DateTimeOffset Date { get; private set; }

    public TransactionHash Hash { get; private set; } = null!;
    
    public CashFlowEvent Event { get; private set; } = null!;

    public CryptoTokenStatistic Token0 { get; private set; } = null!;
}