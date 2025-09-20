using CryptoWatcher.Shared.ValueObjects;

namespace CryptoWatcher.Abstractions;

public interface ICacheFlow
{
    DateTime Date { get; init; }

    CacheFlowEvent Event { get; init; }
}

public interface IUsdCacheFlow : ICacheFlow
{
    decimal Usd { get; init; }
}

public interface ITokenCacheFlow : ICacheFlow
{
    TokenInfo Token { get; init; }
}

public enum CacheFlowEvent
{
    Deposit = 1,
    Withdraw = 2
}