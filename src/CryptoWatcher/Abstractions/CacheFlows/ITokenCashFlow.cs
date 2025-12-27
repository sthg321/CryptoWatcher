using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Abstractions.CacheFlows;

public interface ITokenCashFlow : ICashFlow
{
    CryptoTokenStatistic Token0 { get; }
}