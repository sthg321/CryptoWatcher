using CryptoWatcher.Modules.Hyperliquid.Application.Models;
using CryptoWatcher.Modules.Hyperliquid.Application.Services.PositionUpdates;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Hyperliquid.Application.Abstractions;

public interface IHyperliquidGateway
{
    Task<Queue<VaultUpdate>> GetVaultUpdatesAsync(EvmAddress walletAddress,
        DateTime from, DateTime to,
        CancellationToken ct = default);

    Task<IReadOnlyCollection<HyperliquidVault>> GetVaultsPositionsEquityAsync(EvmAddress walletAddress,
        CancellationToken ct = default);
}