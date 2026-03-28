using CryptoWatcher.Modules.Fluid.Entities.Supply;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Fluid.Abstractions;

public interface IFluidLendPositionRepository
{
    Task<FluidLendPosition?> GetActivePositionAsync(int chainId, EvmAddress fTokenAddress, EvmAddress walletAddress,
        CancellationToken ct = default);
}