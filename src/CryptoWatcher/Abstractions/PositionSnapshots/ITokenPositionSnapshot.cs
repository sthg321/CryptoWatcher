using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Abstractions.PositionSnapshots;

public interface ITokenPositionSnapshot : IPositionSnapshot
{
    CryptoTokenStatistic Token0 { get; }
}