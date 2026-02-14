using CryptoWatcher.Modules.Hyperliquid.Application.Features.Synchronization.VaultSynchronization.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Entities;
using CryptoWatcher.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CryptoWatcher.Modules.Hyperliquid.Infrastructure.Persistence.Repositories;

public class HyperliquidSyncStateRepository : IHyperliquidSyncStateRepository
{
    private readonly HyperliquidDbContext _context;

    public HyperliquidSyncStateRepository(HyperliquidDbContext context)
    {
        _context = context;
    }

    public async Task AddOrUpdateAsync(HyperliquidSynchronizationState state, CancellationToken ct = default)
    {
        await _context.SingleMergeAsync(state, ct);
    }

    public async Task<HyperliquidSynchronizationState?> GetByWalletAsync(EvmAddress wallet,
        CancellationToken ct = default)
    {
        return await _context.HyperliquidSynchronizationStates
            .FirstOrDefaultAsync(state => state.WalletAddress == wallet, ct);
    }
}