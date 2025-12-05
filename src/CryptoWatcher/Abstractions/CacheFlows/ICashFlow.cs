namespace CryptoWatcher.Abstractions.CacheFlows;

public interface ICashFlow
{
    DateTime Date { get; init; }

    CashFlowEvent Event { get; init; }
}