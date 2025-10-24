using CryptoWatcher.Exceptions;
using CryptoWatcher.Extensions;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Hyperliquid.Entities;

public class HyperliquidDailyBalanceChange
{
    private HyperliquidDailyBalanceChange()
    {
    }

    public DateOnly Day { get; private set; }

    public EvmAddress VaultAddress { get; private set; } = null!;

    public EvmAddress WalletAddress { get; private set; } = null!;

    public decimal DailyChange { get; private set; }

    public static HyperliquidDailyBalanceChange CreateFromSnapshot(
        HyperliquidVaultPosition position,
        HyperliquidVaultPositionSnapshot previousSnapshot,
        HyperliquidVaultPositionSnapshot currentSnapshot)
    {
        if (!previousSnapshot.VaultAddress.Equals(position.VaultAddress) ||
            !previousSnapshot.WalletAddress.Equals(position.WalletAddress))
        {
            throw new DomainException("Snapshots must belong to the same vault and wallet");
        }

        if (previousSnapshot.Day >= currentSnapshot.Day)
        {
            throw new DomainException("Previous snapshot must be before current");
        }

        return new HyperliquidDailyBalanceChange
        {
            Day = currentSnapshot.Day,
            VaultAddress = currentSnapshot.VaultAddress,
            WalletAddress = currentSnapshot.WalletAddress,
            DailyChange = position.CalculateProfitInUsd(previousSnapshot.Day, currentSnapshot.Day).Amount
        };
    }
}