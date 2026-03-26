using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Fluid.Entities.Supply;

public class FluidLendPositionSnapshot
{
    public Guid FluidSupplyPositionId { get; set; }

    public CryptoTokenStatistic Token { get; set; } = null!;

    public DateOnly Day { get; set; }
}