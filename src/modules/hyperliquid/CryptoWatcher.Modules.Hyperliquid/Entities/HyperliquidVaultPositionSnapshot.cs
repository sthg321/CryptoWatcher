using CryptoWatcher.Abstractions.PositionSnapshots;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Hyperliquid.Entities;

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
    public EvmAddress VaultAddress { get; init; } = null!; 
    
    /// <summary>
    /// Represents the wallet address associated with the liquidity pool position.
    /// </summary>
    /// <remarks>
    /// This property holds the blockchain wallet address linked to the liquidity pool position.
    /// It is used to identify the owner of the position and manage the related account details.
    /// </remarks>
    public EvmAddress WalletAddress { get; init; } = null!;
}