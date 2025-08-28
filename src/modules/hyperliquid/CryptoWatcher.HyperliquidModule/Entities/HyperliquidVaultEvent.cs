using System.ComponentModel.DataAnnotations;

namespace CryptoWatcher.Entities.Hyperliquid;

public class HyperliquidVaultEvent
{
    public decimal Usd { get; init; }

    public DateTime Date { get; init; }
    
    public HyperliquidVaultVaultEventType EventType { get; init; }

    [MaxLength(64)]
    public string VaultAddress { get; init; } = null!;

    /// <summary>
    /// Represents the wallet address associated with the liquidity pool position.
    /// </summary>
    /// <remarks>
    /// This property holds the blockchain wallet address linked to the liquidity pool position.
    /// It is used to identify the owner of the position and manage the related account details.
    /// </remarks>
    [MaxLength(64)]
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