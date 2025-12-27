using CryptoWatcher.Abstractions;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Morpho.Entities;

public class MorphoPosition : IDeFiPosition<MorphoPositionSnapshot, MorphoPositionCashFlow>
{
    private readonly List<MorphoPositionCashFlow> _cashFlows = [];

    private readonly List<MorphoPositionSnapshot> _positionSnapshots = [];

    private readonly List<MorphoPositionPeriod> _positionPeriods = [];

    public CryptoToken Token0 { get; private set; } = null!;

    public IReadOnlyCollection<MorphoPositionSnapshot> PositionSnapshots => _positionSnapshots;

    public IReadOnlyCollection<MorphoPositionCashFlow> CashFlows => _cashFlows;

    public IReadOnlyCollection<MorphoPositionPeriod> Periods => _positionPeriods;
}