using CryptoWatcher.Shared.ValueObjects;

namespace CryptoWatcher.Abstractions.PositionSnapshots;

public interface ITokenPositionSnapshot : IPositionSnapshot
{
    TokenInfo Token { get; }
}