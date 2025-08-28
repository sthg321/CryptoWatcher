using System.Numerics;
using CryptoWatcher.Models;
using UniswapClient.Models;

namespace CryptoWatcher.Core;
// ReSharper disable InconsistentNaming
/// <summary>
/// Provides mathematical operations and utilities for Uniswap liquidity pools and positions.
/// </summary>
public interface IUniswapMath
{
    /// <summary>
    /// Calculates the token amounts for given liquidity within a liquidity pool,
    /// based on the current and range price boundaries.
    /// </summary>
    /// <param name="sqrtRatioX96">The current square root price Q64.96 format.</param>
    /// <param name="sqrtRatioAX96">The lower square root price boundary Q64.96 format.</param>
    /// <param name="sqrtRatioBX96">The upper square root price boundary Q64.96 format.</param>
    /// <param name="liquidity">The liquidity value to calculate the token amounts for.</param>
    /// <returns>A tuple containing the calculated token amounts (amount0, amount1).</returns>
    (BigInteger amount0, BigInteger amount1) CalculateTokenAmounts(
        BigInteger sqrtRatioX96,
        BigInteger sqrtRatioAX96,
        BigInteger sqrtRatioBX96,
        BigInteger liquidity);

    /// <summary>
    /// Calculates the position details within a specified liquidity pool based on the given token position and pool data.
    /// </summary>
    /// <param name="pool">The liquidity pool containing the current state of the pool.</param>
    /// <param name="position">The Uniswap position information of the token, including its bounds and liquidity.</param>
    /// <returns>An instance of PositionInPool containing the position ID, associated token pair information, and range status.</returns>
    PositionInPool CalculatePosition(LiquidityPool pool, IUniswapPosition position);

    /// <summary>
    /// Calculates the claimable fee for a given liquidity pool position, based on the current and range price boundaries..
    /// </summary>
    /// <param name="pool">The liquidity pool object containing the current state of the pool.</param>
    /// <param name="position">The Uniswap position associated with the liquidity pool.</param>
    /// <returns>A <see cref="TokenPair"/> object representing the claimable fees for token0 and token1.</returns>
    TokenPair CalculateClaimableFee(LiquidityPool pool, IUniswapPosition position);
}