using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Abstractions.PositionSnapshots;

namespace CryptoWatcher.Abstractions;

public interface IDeFiPosition<out TSnapshot, out TCashFlow>
    where TSnapshot : IPositionSnapshot
    where TCashFlow : ICashFlow
{
    IReadOnlyCollection<TCashFlow> CashFlows { get; }

    IReadOnlyCollection<TSnapshot> PositionSnapshots { get; }
}