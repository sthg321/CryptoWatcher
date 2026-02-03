using CryptoWatcher.Modules.Hyperliquid.Application.Services.PositionUpdates;
using CryptoWatcher.Modules.Hyperliquid.Entities;

namespace CryptoWatcher.Modules.Hyperliquid.Application.Abstractions;

public interface IHyperliquidPositionUpdateApplier
{
    HyperliquidVaultPosition ApplyUpdate(HyperliquidVaultPosition? position, VaultUpdate update);
}