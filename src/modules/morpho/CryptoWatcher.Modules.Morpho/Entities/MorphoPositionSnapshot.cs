using CryptoWatcher.Abstractions.PositionSnapshots;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Morpho.Entities;

public class MorphoPositionSnapshot : ITokenPositionSnapshot
{
    public DateOnly Day { get; private set; }
    
    public CryptoTokenStatistic Token0 { get; private set; }
}