using CryptoWatcher.Abstractions.PositionSnapshots;
using CryptoWatcher.Exceptions;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Entities;

public class UniswapLiquidityPositionSnapshot : ITokenPairPositionSnapshot
{
    private UniswapLiquidityPositionSnapshot()
    {
    }

    public UniswapLiquidityPositionSnapshot(UniswapLiquidityPosition position, DateOnly day, bool isInRange,
        CryptoTokenStatisticWithFee token0, CryptoTokenStatisticWithFee token1)
    {
        PoolPositionId = position.PositionId;
        NetworkName = position.NetworkName;
        Day = day;
        IsInRange = isInRange;
        Token0 = token0;
        Token1 = token1;
    }

    /// <summary>
    /// Represents the unique identifier for a liquidity pool position from NFT manager.
    /// </summary>
    /// <remarks>
    /// This property is used to uniquely identify a specific position within the liquidity pool.
    /// It serves as a key for referencing and managing individual positions across the system.
    /// </remarks>
    public ulong PoolPositionId { get; private set; }

    /// <summary>
    /// Specifies the name of the uniswapNetwork associated with the current configuration or operation.
    /// </summary>
    /// <remarks>
    /// This property is used to identify the uniswapNetwork, such as a blockchain or communication uniswapNetwork,
    /// that the system is interacting with or utilizing for its processes.
    /// </remarks>
    public string NetworkName { get; private set; } = null!;

    /// <summary>
    /// Represents the date when the liquidity pool position was created.
    /// </summary>
    /// <remarks>
    /// This property stores the creation date for a specific position in the liquidity pool.
    /// It is used to track the timeline of when the position was initialized.
    /// </remarks>
    public DateOnly Day { get; private set; }

    /// <summary>
    /// Indicates whether a liquidity pool position is currently within its specified price range.
    /// </summary>
    /// <remarks>
    /// This property determines if the liquidity position falls within the active price range
    /// for token trading in the pool. It is used for assessing a position's eligibility
    /// for earning fees and contributing liquidity under current market conditions.
    /// </remarks>
    public bool IsInRange { get; private set; }

    /// <summary>
    /// Represents the metadata and fee details of the first token in a Uniswap liquidity pool position.
    /// </summary>
    /// <remarks>
    /// This property holds information about the first token (Token0) in a liquidity pool,
    /// including its symbol, amount, fee amount, and price in USD. It is used to calculate
    /// the total value and fee-related metrics of the position for reporting and data analysis.
    /// </remarks>
    public CryptoTokenStatisticWithFee Token0 { get; private set; } = null!;

    /// <summary>
    /// Represents the secondary token of a liquidity pool position along with its associated fee details.
    /// </summary>
    /// <remarks>
    /// This property provides information about the second token in the liquidity pool.
    /// It includes details such as the token's symbol, amount, fee amount, and price in USD.
    /// This data is critical for accurately calculating the position’s overall performance and associated fees.
    /// </remarks>
    public CryptoTokenStatisticWithFee Token1 { get; private set; } = null!;

    /// <summary>
    /// Represents the total fee earned in USD for a specific pool position snapshot.
    /// </summary>
    /// <remarks>
    /// This property calculates the combined value of fees earned by both tokens in the pool position,
    /// based on their respective fee amounts and prices in USD. It provides a monetary representation
    /// of the fees collected within the snapshot timeframe.
    /// </remarks>
    public decimal FeeInUsd => Token0.FeeInUsd + Token1.FeeInUsd;

    public decimal AmountInUsd => Token0.AmountInUsd + Token1.AmountInUsd;

    /// <summary>
    /// Calculates the total sum in USD of tokens held within the position snapshot by combining the USD values of token0 and token1.
    /// </summary>
    /// <returns>
    /// A decimal value representing the total USD value of token0 and token1 for the position snapshot.
    /// </returns>
    public decimal TokenSumInUsd() => Token0.AmountInUsd + Token1.AmountInUsd;

    public void Update(CryptoTokenStatisticWithFee token0, CryptoTokenStatisticWithFee token1, bool isInRange)
    {
        IsInRange = isInRange;
        Token0 = token0;
        Token1 = token1;
    }
}