using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Abstractions.CacheFlows;

public interface ITokenCashFlow : ICashFlow
{
    CryptoToken CryptoToken { get; init; }
}