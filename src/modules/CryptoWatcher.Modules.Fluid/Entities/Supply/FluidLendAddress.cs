using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Fluid.Entities.Supply;

public class FluidLendAddress
{
    public int ChainId { get; init; }

    public EvmAddress Address { get; init; } = null!;
} 