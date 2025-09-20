using CryptoWatcher.Shared.ValueObjects;

namespace CryptoWatcher.Abstractions;

public interface IPositionSnapshot
{
    DateOnly Day { get; init; }
}

public interface IUsdPositionSnapshot : IPositionSnapshot
{
    decimal GetUsdBalance();
}

public interface ITokenPositionSnapshot : IPositionSnapshot
{
    TokenInfo GetTokenInfo();
}