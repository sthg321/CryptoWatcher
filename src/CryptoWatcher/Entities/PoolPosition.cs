using CryptoWatcher.Entities.Uniswap;

namespace CryptoWatcher.Entities;

/// <summary>
/// Represents a position in a liquidity pool, tracking the amounts and USD values of tokens,
/// as well as associated metadata such as creation date, uniswapNetwork information, and status.
/// </summary>
/// <remarks>
/// A liquidity pool position contains details about token quantities, their equivalent USD values,
/// and the associated blockchain uniswapNetwork. This class also includes information on whether the
/// position is currently active and the date it was created.
/// </remarks>
public class PoolPosition
{
    /// <summary>
    /// Represents the unique identifier for a liquidity pool position from NFT manager.
    /// </summary>
    /// <remarks>
    /// This property is used to uniquely identify a specific position within the liquidity pool.
    /// It serves as a key for referencing and managing individual positions across the system.
    /// </remarks>
    public ulong PositionId { get; init; }

    /// <summary>
    /// Represents the date when the liquidity pool position was created.
    /// </summary>
    /// <remarks>
    /// This property stores the creation date for a specific position in the liquidity pool.
    /// It is used to track the timeline of when the position was initialized.
    /// </remarks>
    public DateOnly SynchronizedAt { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    public TokenInfo Token0 { get; init; } = null!;
    
    /// <summary>
    /// 
    /// </summary>
    public TokenInfo Token1 { get; init; } = null!;

    /// <summary>
    /// Indicates whether the liquidity pool position is active.
    /// </summary>
    /// <remarks>
    /// This property is used to determine the current status of the position.
    /// An active state signifies that the position is engaged in the liquidity pool,
    /// while an inactive state suggests the position has been exited or is no longer valid.
    /// </remarks>
    public bool IsActive { get; init; }
    
    /// <summary>
    /// Indicates whether the liquidity pool position is within the specified range for the pool's price bounds.
    /// </summary>
    /// <remarks>
    /// This property determines whether the current liquidity position is active within the operational
    /// range of the liquidity pool, which may depend on token price bounds or other parameters tied
    /// to the pool's configuration. It assists in assessing the position's current applicability or utility.
    /// </remarks>
    public bool IsInRange { get; init; }

    /// <summary>
    /// Represents the wallet address associated with the liquidity pool position.
    /// </summary>
    /// <remarks>
    /// This property holds the blockchain wallet address that is linked to the liquidity pool position.
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
    
    /// <summary>
    /// Specifies the name of the uniswapNetwork associated with the current configuration or operation.
    /// </summary>
    /// <remarks>
    /// This property is used to identify the uniswapNetwork, such as a blockchain or communication uniswapNetwork,
    /// that the system is interacting with or utilizing for its processes.
    /// </remarks>
    public string NetworkName { get; init; } = null!;

    /// <summary>
    /// Represents the blockchain uniswapNetwork associated with the liquidity pool position.
    /// </summary>
    /// <remarks>
    /// This property defines the uniswapNetwork on which the liquidity pool position operates. It provides
    /// details and configurations specific to the blockchain, such as RPC URL, associated addresses,
    /// and historical data, aiding in uniswapNetwork-specific operations and analytics.
    /// </remarks>
    public UniswapNetwork UniswapNetwork { get; init; } = null!;

    /// <summary>
    /// Represents the collection of historical records for a liquidity pool position.
    /// </summary>
    /// <remarks>
    /// This property contains a list of historical snapshots related to the associated liquidity pool position.
    /// Each record includes token quantities, USD values, fees, and other relevant details for a specific date.
    /// It provides a comprehensive history of the position's performance and activity over time.
    /// </remarks>
    public List<PositionFee> PositionFees { get; set; } = [];
}