using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Abstractions.CacheFlows;

public interface ITokenPairCashFlow : ICashFlow
{
    TokenInfoWithFee Token0 { get; set; }
    
    TokenInfoWithFee Token1 { get; set; }
}