using System.Numerics;

namespace UniswapClient.Models;

/// <summary>
/// Represents the state of an Uniswap liquidity pool.
/// </summary>
public class LiquidityPoolInfo
{
    /// <summary>
    /// Current tick of the pool.
    /// </summary>
    public required int Tick { get; init; }

    /// <summary>
    /// Current square root price in Q64.96 format.
    /// </summary>
    public required BigInteger SqrtPriceX96 { get; init; }

    /// <summary>
    /// Global fee growth for token0 (in Q128.128 format).
    /// </summary>
    public required BigInteger FeeGrowthGlobal0X128 { get; init; }

    /// <summary>
    /// Global fee growth for token1 (in Q128.128 format).
    /// </summary>
    public required BigInteger FeeGrowthGlobal1X128 { get; init; }

    /// <summary>
    /// Data for the lower tick of a specific position.
    /// </summary>
    public required PoolTickInfo LowerTick { get; init; } = null!;

    /// <summary>
    /// Data for the upper tick of a specific position.
    /// </summary>
    public required PoolTickInfo UpperTick { get; init; } = null!;
}