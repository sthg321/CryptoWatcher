namespace CryptoWatcher.Abstractions.CacheFlows;

public interface ICashFlow
{
    DateTime Date { get; }

    CashFlowEvent Event { get; }
}