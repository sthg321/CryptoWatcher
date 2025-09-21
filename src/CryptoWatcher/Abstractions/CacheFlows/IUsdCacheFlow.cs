namespace CryptoWatcher.Abstractions.CacheFlows;

public interface IUsdCacheFlow : ICacheFlow
{
    decimal Usd { get; init; }
}