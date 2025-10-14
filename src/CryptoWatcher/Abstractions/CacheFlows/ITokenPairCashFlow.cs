using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Abstractions.CacheFlows;

public interface ITokenPairCashFlow : ICacheFlow
{
    TokenInfoWithFee Token0 { get; set; }
    
    TokenInfoWithFee Token1 { get; set; }
}