using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Merkl.Application.Abstractions;

public interface IMerklSyncService
{
    Task SyncRewardsAsync(EvmAddress walletAddress, int chainId,
        DateOnly day,
        CancellationToken ct = default);
}