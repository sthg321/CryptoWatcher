using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Entities;
using CryptoWatcher.Modules.Hyperliquid.Specifications;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Hyperliquid.Application.Services;

public class HyperliquidVaultPositionSyncJob
{
    private readonly IRepository<HyperliquidVaultPosition> _repository;
    private readonly HyperliquidVaultPositionUpdater _positionUpdater;

    public HyperliquidVaultPositionSyncJob(IRepository<HyperliquidVaultPosition> repository,
        HyperliquidVaultPositionUpdater positionUpdater)
    {
        _repository = repository;
        _positionUpdater = positionUpdater;
    }

    public async Task SyncPositionAsync(EvmAddress walletAddress, DateOnly from, DateOnly to, CancellationToken ct = default)
    {
        var position = await _repository.FirstOrDefaultAsync(
            new HyperliquidPositionsWithSnapshotsAndCashFlowByWallet(walletAddress, from, to), ct);

        var updatedPosition = await _positionUpdater.UpdatePositionAsync(position, walletAddress, from, to, ct);

        if (updatedPosition is not null)
        {
            await _repository.BulkMergeAsync([updatedPosition], ct);
        }
    }
}