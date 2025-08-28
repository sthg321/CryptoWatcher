using System.Numerics;

namespace CryptoWatcher.UniswapModule.Models;

/// <summary>
/// Represents fee growth data for a specific tick boundary in a liquidity pool.
/// </summary>
public record LiquidityPoolTick
{
    /// <summary>
    /// Total fee growth outside this tick for token0 (in Q128.128 format).
    /// </summary>
    public required BigInteger FeeGrowthOutside0X128 { get; init; }

    /// <summary>
    /// Total fee growth outside this tick for token1 (in Q128.128 format).
    /// </summary>
    public required BigInteger FeeGrowthOutside1X128 { get; init; }
}