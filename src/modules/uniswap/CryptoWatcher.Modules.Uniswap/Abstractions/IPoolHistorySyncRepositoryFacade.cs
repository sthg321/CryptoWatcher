using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Uniswap.Abstractions;

/// <summary>
/// Interface for managing and retrieving data related to pool history synchronization features.
/// </summary>
public interface IPoolHistorySyncRepositoryFacade
{
    /// <summary>
    /// Asynchronously retrieves a list of liquidity pool positions for the specified uniswapNetwork and wallet.
    /// </summary>
    /// <param name="chainConfiguration">The blockchain uniswapNetwork for which liquidity pool positions are being retrieved.</param>
    /// <param name="wallet">The wallet for which liquidity pool positions are being retrieved.</param>
    /// <param name="ct">An optional cancellation token to cancel the operation if required.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="UniswapLiquidityPosition"/> objects representing the liquidity pool positions.</returns>
    Task<List<UniswapLiquidityPosition>> GetLiquidityPoolPositionsAsync(UniswapChainConfiguration chainConfiguration, Wallet wallet,
        CancellationToken ct = default);

    /// <summary>
    /// Asynchronously merges a list of liquidity pool positions with corresponding snapshots.
    /// </summary>
    /// <param name="positions">A collection of <see cref="UniswapLiquidityPosition"/> entities representing the pool positions to merge.</param>
    /// <param name="snapshots">A collection of <see cref="UniswapLiquidityPositionSnapshot"/> entities representing the snapshots to merge with the positions.</param>
    /// <param name="ct">An optional cancellation token to cancel the operation if required.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task MergePoolPositionsAsync(IList<UniswapLiquidityPosition> positions,
        IList<UniswapLiquidityPositionSnapshot> snapshots,
        CancellationToken ct = default);
}