using CryptoWatcher.Modules.Hyperliquid.Application.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Entities;

namespace CryptoWatcher.Infrastructure;

public class HyperliquidSyncRepoFacade : IHyperliquidSyncRepoFacade
{
    private readonly CryptoWatcherDbContext _context;

    public HyperliquidSyncRepoFacade(CryptoWatcherDbContext context)
    {
        _context = context;
    }

    public async Task SavePositionWithGraphAsync(IReadOnlyCollection<HyperliquidVaultPosition> vaults,
        CancellationToken ct = default)
    {
        await _context.BulkMergeAsync(vaults, ct);

        await _context.BulkMergeAsync(vaults.SelectMany(vault => vault.PositionSnapshots), ct);
        await _context.BulkMergeAsync(vaults.SelectMany(vault => vault.CashFlows), ct);
    }
}