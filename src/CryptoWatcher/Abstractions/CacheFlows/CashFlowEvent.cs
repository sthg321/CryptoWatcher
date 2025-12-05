using Ardalis.SmartEnum;

namespace CryptoWatcher.Abstractions.CacheFlows;

public class CashFlowEvent : SmartEnum<CashFlowEvent>
{
    private CashFlowEvent(string name, int value) : base(name, value) { }

    public static readonly CashFlowEvent Deposit = new(nameof(Deposit), 1);
    
    public static readonly CashFlowEvent Withdrawal = new(nameof(Withdrawal), 2);
    
    public static readonly CashFlowEvent FeeClaim = new(nameof(FeeClaim), 3);
}