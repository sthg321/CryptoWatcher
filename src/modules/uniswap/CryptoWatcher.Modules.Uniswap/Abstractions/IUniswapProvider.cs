using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Models;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Uniswap.Abstractions;

/// <summary>
/// Defines methods for interacting with Uniswap to retrieve and manage Uniswap positions and liquidity pools.
/// </summary>
public interface IUniswapProvider
{
    /// <summary>
    /// Retrieves a list of Uniswap positions associated with the specified network and wallet.
    /// </summary>
    /// <param name="chainConfiguration">The Uniswap network to query for positions.</param>
    /// <param name="tokenIds">Nft ids</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a list of Uniswap positions associated with the specified network and wallet.</returns>
    Task<List<IUniswapPosition>> GetPositionsAsync(UniswapChainConfiguration chainConfiguration,
        List<ulong> tokenIds);

    /// <summary>
    /// Retrieves the liquidity pool associated with the specified Uniswap network and position.
    /// </summary>
    /// <param name="chainConfiguration">The Uniswap network from which to retrieve the liquidity pool.</param>
    /// <param name="position">The position details used to identify the targeted liquidity pool.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the liquidity pool associated with the specified network and position.</returns>
    Task<LiquidityPool> GetPoolAsync(UniswapChainConfiguration chainConfiguration, IUniswapPosition position);
}