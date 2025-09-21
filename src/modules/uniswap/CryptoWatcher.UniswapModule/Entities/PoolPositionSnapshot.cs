using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.PositionSnapshots;

namespace CryptoWatcher.UniswapModule.Entities;

public class PoolPositionSnapshot : IPositionSnapshot
{
    /// <summary>
    /// Represents the unique identifier for a liquidity pool position from NFT manager.
    /// </summary>
    /// <remarks>
    /// This property is used to uniquely identify a specific position within the liquidity pool.
    /// It serves as a key for referencing and managing individual positions across the system.
    /// </remarks>
    public ulong PoolPositionId { get; init; }

    /// <summary>
    /// Specifies the name of the uniswapNetwork associated with the current configuration or operation.
    /// </summary>
    /// <remarks>
    /// This property is used to identify the uniswapNetwork, such as a blockchain or communication uniswapNetwork,
    /// that the system is interacting with or utilizing for its processes.
    /// </remarks>
    public string NetworkName { get; init; } = null!;
    
    /// <summary>
    /// Represents the date when the liquidity pool position was created.
    /// </summary>
    /// <remarks>
    /// This property stores the creation date for a specific position in the liquidity pool.
    /// It is used to track the timeline of when the position was initialized.
    /// </remarks>
    public DateOnly Day { get; init; }

    /// <summary>
    /// Indicates whether a liquidity pool position is currently within its specified price range.
    /// </summary>
    /// <remarks>
    /// This property determines if the liquidity position falls within the active price range
    /// for token trading in the pool. It is used for assessing a position's eligibility
    /// for earning fees and contributing liquidity under current market conditions.
    /// </remarks>
    public bool IsInRange { get; init; }

    /// <summary>
    /// Represents the metadata and fee details of the first token in a Uniswap liquidity pool position.
    /// </summary>
    /// <remarks>
    /// This property holds information about the first token (Token0) in a liquidity pool,
    /// including its symbol, amount, fee amount, and price in USD. It is used to calculate
    /// the total value and fee-related metrics of the position for reporting and data analysis.
    /// </remarks>
    public TokenInfoWithFee Token0 { get; init; } = null!;

    /// <summary>
    /// Represents the secondary token of a liquidity pool position along with its associated fee details.
    /// </summary>
    /// <remarks>
    /// This property provides information about the second token in the liquidity pool.
    /// It includes details such as the token's symbol, amount, fee amount, and price in USD.
    /// This data is critical for accurately calculating the position’s overall performance and associated fees.
    /// </remarks>
    public TokenInfoWithFee Token1 { get; init; } = null!;

    /// <summary>
    /// Represents the liquidity pool position associated with the snapshot.
    /// </summary>
    /// <remarks>
    /// This property links a specific pool position to its corresponding snapshot, enabling a detailed
    /// view of token balances, fees, and other relevant data for that particular position at a given point in time.
    /// It plays a critical role in connecting historical data to the underlying liquidity pool position.
    /// </remarks>
    public PoolPosition PoolPosition { get; init; } = null!;

    /// <summary>
    /// Represents the total fee earned in USD for a specific pool position snapshot.
    /// </summary>
    /// <remarks>
    /// This property calculates the combined value of fees earned by both tokens in the pool position,
    /// based on their respective fee amounts and prices in USD. It provides a monetary representation
    /// of the fees collected within the snapshot timeframe.
    /// </remarks>
    public decimal FeeInUsd => Token0.FeeAmount * Token0.PriceInUsd + Token1.FeeAmount * Token1.PriceInUsd;

    /// <summary>
    /// Calculates the total sum in USD of tokens held within the position snapshot by combining the USD values of token0 and token1.
    /// </summary>
    /// <returns>
    /// A decimal value representing the total USD value of token0 and token1 for the position snapshot.
    /// </returns>
    public decimal TokenSumInUsd() => Token0.AmountInUsd + Token1.AmountInUsd;
}