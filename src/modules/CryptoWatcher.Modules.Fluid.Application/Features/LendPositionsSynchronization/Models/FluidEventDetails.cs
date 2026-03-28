using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Fluid.Application.Features.LendPositionsSynchronization.Models;

public class FluidEventDetails
{
    public FluidEvent Event { get; set; } = null!;

    public int ChainId { get; set; }

    public EvmAddress WalletAddress { get; set; } = null!;

    public TransactionHash Hash { get; set; } = null!;
    
    public DateTimeOffset Timestamp { get; set; }
}