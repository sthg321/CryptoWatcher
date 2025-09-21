namespace CryptoWatcher.Abstractions.CacheFlows;

public interface ICacheFlow
{
    DateTime Date { get; init; }

    CacheFlowEvent Event { get; init; }
}