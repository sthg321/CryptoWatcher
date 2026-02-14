using CryptoWatcher.Modules.Hyperliquid.Application.Features.Synchronization.VaultSynchronization.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Entities;

namespace CryptoWatcher.Modules.Hyperliquid.Application.Features.Synchronization.VaultSynchronization;

public class HyperliquidSnapshotsUpdater : IHyperliquidSnapshotsUpdater
{
    private readonly IHyperliquidGateway _gateway;

    public HyperliquidSnapshotsUpdater(IHyperliquidGateway gateway)
    {
        _gateway = gateway;
    }

    public async Task TakeVaultBalanceSnapshotAsync(HyperliquidVaultPosition position,
        DateTime syncDate,
        CancellationToken ct = default)
    {
        var vault = (await _gateway.GetVaultsPositionsEquityAsync(position.WalletAddress, ct))
            .FirstOrDefault(hyperliquidVault =>
                hyperliquidVault.Address.Equals(HyperliquidWellKnowFields.HlpVaultAddress));

        if (vault is null) // for case to sync old positions. For them, we can't take snapshot
        {
             return;
        }

        position.AddOrUpdateSnapshot(vault.Balance, syncDate);
    }
}