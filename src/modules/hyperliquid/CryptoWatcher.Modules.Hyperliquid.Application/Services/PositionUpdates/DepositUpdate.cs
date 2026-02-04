using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Hyperliquid.Application.Services.PositionUpdates;

public class DepositUpdate : VaultUpdate
{
    public EvmAddress WalletAddress { get; set; } = null!;
}