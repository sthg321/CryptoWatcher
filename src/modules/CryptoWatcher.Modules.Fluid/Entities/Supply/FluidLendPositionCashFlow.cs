using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Fluid.Entities.Supply;

public class FluidLendPositionCashFlow
{
    private FluidLendPositionCashFlow()
    {
        
    }
    
    public FluidLendPositionCashFlow(DateTimeOffset date, TransactionHash hash, CashFlowEvent @event,
        CryptoTokenStatistic token0)
    {
        Date = date;
        Hash = hash;
        Event = @event;
        Token0 = token0;
    }

    public DateTimeOffset Date { get; private set; }

    public TransactionHash Hash { get; private set; } = null!;

    public CashFlowEvent Event { get; private set; } = null!;

    public CryptoTokenStatistic Token0 { get; private set; } = null!;
}