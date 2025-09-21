using CryptoWatcher.Shared.ValueObjects;

namespace CryptoWatcher.Abstractions.CacheFlows;

public interface ITokenCacheFlow : ICacheFlow
{
    TokenInfo Token { get; init; }
}