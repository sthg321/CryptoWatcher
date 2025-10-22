using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Hyperliquid.Entities;

/// <summary>
/// Represents an event associated with a Hyperliquid vault. These events track interactions such as deposits or withdrawals on specific vaults
/// and are linked to a specific wallet within the Hyperliquid module.
/// </summary>
public class HyperliquidVaultEvent : IUsdCacheFlow
{
    /// <summary>
    /// Represents the monetary value in USD associated with a vault-related activity.
    /// </summary>
    /// <remarks>
    /// This property encapsulates the financial amount in US dollars for a specific activity,
    /// such as deposits or withdrawals, in the context of operations involving Hyperliquid vaults.
    /// It is integral to tracking transactional data and maintaining a record of vault-related events.
    /// </remarks>
    public decimal Usd { get; init; }

    /// <summary>
    /// Represents the date and time associated with a specific Hyperliquid vault event.
    /// </summary>
    /// <remarks>
    /// This property captures the timestamp marking the occurrence of an event within the Hyperliquid vault system,
    /// such as deposits or withdrawals. It is a critical component in identifying and organizing event records,
    /// enabling chronological tracking and data management of vault-related activities.
    /// </remarks>
    public DateTime Date { get; init; }
    
    /// <summary>
    /// Indicates the type of event associated with a Hyperliquid vault activity.
    /// </summary>
    /// <remarks>
    /// This property specifies the nature of a vault-related event, such as a deposit or withdrawal,
    /// within the Hyperliquid platform. It is used to classify and track the different types of interactions
    /// occurring in the context of vault operations.
    /// </remarks>
    public CacheFlowEvent Event { get; init; } = null!;

    /// <summary>
    /// Represents the unique address identifier for a Hyperliquid vault.
    /// </summary>
    /// <remarks>
    /// This property stores the address string that uniquely identifies a specific vault in the Hyperliquid platform.
    /// It plays a vital role in tracking and associating vault-related activities such as deposits, withdrawals,
    /// and other events within the system.
    /// </remarks>
    public EvmAddress VaultAddress { get; init; } = null!;

    /// <summary>
    /// Represents the wallet address associated with the liquidity pool position.
    /// </summary>
    /// <remarks>
    /// This property holds the blockchain wallet address linked to the liquidity pool position.
    /// It is used to identify the owner of the position and manage the related account details.
    /// </remarks>
    public EvmAddress WalletAddress { get; init; } = null!;

    /// <summary>
    /// Represents the wallet associated with a liquidity pool position.
    /// </summary>
    /// <remarks>
    /// This property identifies the wallet that holds ownership of the liquidity pool position.
    /// It includes the wallet's unique identifier and blockchain address for managing assets.
    /// </remarks>
    public Wallet Wallet { get; init; } = null!;
}