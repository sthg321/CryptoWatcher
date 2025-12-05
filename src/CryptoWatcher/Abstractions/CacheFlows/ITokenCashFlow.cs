using CryptoWatcher.Shared.ValueObjects;

namespace CryptoWatcher.Abstractions.CacheFlows;

public interface ITokenCashFlow : ICashFlow
{
    TokenInfo Token { get; init; }
}