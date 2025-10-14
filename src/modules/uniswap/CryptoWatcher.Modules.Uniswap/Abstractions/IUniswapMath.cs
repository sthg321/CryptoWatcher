using CryptoWatcher.Modules.Uniswap.Models;
using CryptoWatcher.Shared.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Abstractions;

/// <summary>
/// Provides mathematical operations and utilities for Uniswap liquidity pools and positions.
/// </summary>
public interface IUniswapMath
{
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