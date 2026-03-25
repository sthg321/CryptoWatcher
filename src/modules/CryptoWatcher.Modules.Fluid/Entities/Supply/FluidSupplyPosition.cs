using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Fluid.Entities.Supply;

public class FluidSupplyPosition
{
    public Guid Id { get; set; }
    
    public int ChainId { get; set; }
    
    public CryptoTokenShort Token { get; set; } = null!;

    public EvmAddress WalletAddress { get; set; } = null!;

    public IReadOnlyCollection<FluidSupplyPositionSnapshot> PositionSnapshots { get; set; } = [];
}