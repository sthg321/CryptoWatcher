using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Fluid.Application.Features.LendPositionsSynchronization.Models;

public class FluidEventDetails
{
    public required FluidEvent Event { get; init; } 
    
    public required int ChainId { get; init; }

    public required EvmAddress WalletAddress { get; init; } = null!;

    public required TransactionHash Hash { get; init; } = null!;
    
    public required DateTimeOffset Timestamp { get; init; }
}