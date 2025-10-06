using CryptoWatcher.Shared.Entities;
using CryptoWatcher.UniswapModule.Entities;
using CryptoWatcher.UniswapModule.Models;
using UniswapClient.Models;

namespace CryptoWatcher.UniswapModule.Abstractions;

/// <summary>
/// Defines methods for interacting with Uniswap to retrieve and manage Uniswap positions and liquidity pools.
/// </summary>
public interface IUniswapProvider
{
    /// <summary>
    /// Retrieves a list of Uniswap positions associated with the specified network and wallet.
    /// </summary>
    /// <param name="uniswapNetwork">The Uniswap network to query for positions.</param>
    /// <param name="wallet">The wallet containing the positions to be retrieved.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a list of Uniswap positions associated with the specified network and wallet.</returns>
    Task<List<IUniswapPosition>> GetPositionsAsync(UniswapNetwork uniswapNetwork, Wallet wallet);

    /// <summary>
    /// Retrieves the liquidity pool associated with the specified Uniswap network and position.
    /// </summary>
    /// <param name="uniswapNetwork">The Uniswap network from which to retrieve the liquidity pool.</param>
    /// <param name="position">The position details used to identify the targeted liquidity pool.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the liquidity pool associated with the specified network and position.</returns>
    Task<LiquidityPool> GetPoolAsync(UniswapNetwork uniswapNetwork, IUniswapPosition position);
}