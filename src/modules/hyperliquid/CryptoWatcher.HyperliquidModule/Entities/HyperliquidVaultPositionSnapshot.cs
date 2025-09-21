using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.PositionSnapshots;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.HyperliquidModule.Entities;

/// <summary>
/// Represents a snapshot of a position in a Hyperliquid vault, including balance, day of the snapshot,
/// vault details, and associated wallet information.
/// </summary>
public class HyperliquidVaultPositionSnapshot : IUsdPositionSnapshot
{
    /// <summary>
    /// Balance in usd
    /// </summary>
    public decimal Balance { get; init; }
    
    /// <summary>
    /// The day when the snapshot was taken
    /// </summary>
    public DateOnly Day { get; init; }

    public decimal GetUsdBalance() => Balance;

    /// <summary>
    /// VaultAddress address
    /// </summary>
    public string VaultAddress { get; init; } = null!;

    /// <summary>
    /// Represents the Hyperliquid vault associated with the position snapshot.
    /// Provides access to details about the vault, such as events, snapshots, and profit calculations.
    /// </summary>
    public HyperliquidVaultPosition Vault { get; init; } = null!;
    
    /// <summary>
    /// Represents the wallet address associated with the liquidity pool position.
    /// </summary>
    /// <remarks>
    /// This property holds the blockchain wallet address linked to the liquidity pool position.
    /// It is used to identify the owner of the position and manage the related account details.
    /// </remarks>
    public string WalletAddress { get; init; } = null!;

    /// <summary>
    /// Represents the wallet associated with a liquidity pool position.
    /// </summary>
    /// <remarks>
    /// This property identifies the wallet that holds ownership of the liquidity pool position.
    /// It includes the wallet's unique identifier and blockchain address for managing assets.
    /// </remarks>
    public Wallet Wallet { get; init; } = null!;
}