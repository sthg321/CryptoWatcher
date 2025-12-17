using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Abstractions.PositionSnapshots;
using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Abstractions;

public interface IDeFiPosition<out TSnapshot, out TCashFlow>
    where TSnapshot : IPositionSnapshot
    where TCashFlow : ICashFlow
{
    CryptoToken Token0 { get; }
    
    IReadOnlyCollection<TCashFlow> CashFlows { get; }

    IReadOnlyCollection<TSnapshot> PositionSnapshots { get; }
}