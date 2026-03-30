using CryptoWatcher.Modules.Fluid.Abstractions;
using CryptoWatcher.Modules.Fluid.Entities.Supply;
using CryptoWatcher.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CryptoWatcher.Modules.Fluid.Infrastructure.Persistence.Repositories;

public class FluidLendPositionRepository : IFluidLendPositionRepository
{
    private readonly FluidDbContext _context;

    public FluidLendPositionRepository(FluidDbContext context)
    {
        _context = context;
    }

    public async Task<FluidLendPosition?> GetActivePositionAsync(int chainId, EvmAddress fTokenAddress,
        EvmAddress walletAddress,
        CancellationToken ct = default)
    {
        return await _context.FluidSupplyPositions.FirstOrDefaultAsync(position => position.ChainId == chainId &&
            position.Token.Address == fTokenAddress &&
            position.WalletAddress == walletAddress, ct);
    }
}