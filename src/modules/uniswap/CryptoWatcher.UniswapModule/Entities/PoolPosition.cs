using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.UniswapModule.Entities;

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
    /// Represents the first token in the liquidity pool pair for a position.
    /// </summary>
    /// <remarks>
    /// This property holds information about one of the two tokens in the liquidity pool.
    /// It is paired with <c>Token1</c> to define the token pair comprising the pool.
    /// </remarks>
    public TokenInfo Token0 { get; init; } = null!;

    /// <summary>
    /// Represents the second token in a liquidity pool position.
    /// </summary>
    /// <remarks>
    /// This property holds details about the second token involved in the liquidity pool.
    /// It is used in association with <c>Token0</c> to define the token pair within the pool
    /// and their respective attributes, enabling operations such as valuation and tracking.
    /// </remarks>
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
    /// Represents a collection of snapshots associated with a specific liquidity pool position.
    /// </summary>
    /// <remarks>
    /// This property holds historical states or snapshots of a liquidity pool position.
    /// These snapshots can be used to track changes and analyze the evolution of the position over time,
    /// including performance metrics, token balances, and other relevant data.
    /// </remarks>
    public List<PoolPositionSnapshot> PoolPositionSnapshots { get; init; } = [];
}