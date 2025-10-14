using CryptoWatcher.Shared.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Models;

/// <summary>
/// Represents a position within a liquidity pool in the Uniswap protocol.
/// Provides details about the unique position ID, its range status,
/// and token data associated with the liquidity position.
/// </summary>
public class PositionInPool
{
    /// <summary>
    /// Represents the unique identifier for a position within a liquidity pool.
    /// This property is essential for determining and referencing positions,
    /// especially during calculations, state tracking, or operations related
    /// to liquidity management and token balances in the Uniswap protocol.
    /// </summary>
    public required ulong PositionId { get; init; }

    /// <summary>
    /// Indicates whether the position is currently within the active price range of the liquidity pool.
    /// This property determines if the position contributes to the pool's liquidity at the given moment,
    /// based on the current price and the defined boundaries of the position.
    /// </summary>
    public required bool IsInRange { get; init; }

    /// <summary>
    /// Represents detailed information about a pair of tokens within a liquidity position.
    /// This property contains token-specific data, such as token identifiers and their respective balances,
    /// and serves as a cornerstone for tracking and managing token allocations in a liquidity context.
    /// </summary>
    public required TokenPair TokenInfoPair { get; init; } = null!;
}