using Ardalis.SmartEnum;

namespace CryptoWatcher.Abstractions.CacheFlows;

public class CacheFlowEvent : SmartEnum<CacheFlowEvent>
{
    private CacheFlowEvent(string name, int value) : base(name, value)
    {
    }

    public static readonly CacheFlowEvent Deposit = new(nameof(Deposit), 1);

    public static readonly CacheFlowEvent Withdrawal = new(nameof(Withdrawal), 2);

    public static readonly CacheFlowEvent FeeClaim = new(nameof(FeeClaim), 2);
}