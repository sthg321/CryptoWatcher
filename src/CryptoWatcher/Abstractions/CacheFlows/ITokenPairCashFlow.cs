using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Abstractions.CacheFlows;

public interface ITokenPairCashFlow : ICacheFlow
{
    TokenInfoWithFee Token0 { get; init; }
    
    TokenInfoWithFee Token1 { get; init; }
}