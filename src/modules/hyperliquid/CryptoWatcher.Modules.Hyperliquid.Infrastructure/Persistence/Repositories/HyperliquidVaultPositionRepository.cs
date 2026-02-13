using CryptoWatcher.Modules.Hyperliquid.Application.Features.Synchronization.VaultSynchronization.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Entities;
using CryptoWatcher.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CryptoWatcher.Modules.Hyperliquid.Infrastructure.Persistence.Repositories;

public class HyperliquidVaultPositionRepository : IHyperliquidVaultPositionRepository
{
    private readonly HyperliquidDbContext _dbContext;
    private readonly HyperliquidVaultPositionBulkWriter _bulkWriter;
    
    public HyperliquidVaultPositionRepository(HyperliquidDbContext dbContext, HyperliquidVaultPositionBulkWriter bulkWriter)
    {
        _dbContext = dbContext;
        _bulkWriter = bulkWriter;
    }

    public async Task AddOrUpdateAsync(HyperliquidVaultPosition position, CancellationToken ct = default)
    {
        await _bulkWriter.MergeAsync([position], ct);
    }

    public async Task<HyperliquidVaultPosition?> GetByWalletAsync(EvmAddress wallet, CancellationToken ct = default)
    {
        return await _dbContext.HyperliquidVaultPositions
            .Include(position => position.Periods)
            .Include(position => position.Snapshots)
            .Include(position => position.CashFlows)
            .FirstOrDefaultAsync(position => position.WalletAddress == wallet, ct);
    }
}