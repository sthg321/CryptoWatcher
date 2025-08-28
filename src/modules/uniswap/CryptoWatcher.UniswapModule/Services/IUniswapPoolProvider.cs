using CryptoWatcher.UniswapModule.Entities;
using CryptoWatcher.UniswapModule.Models;
using Nethereum.Web3;
using UniswapClient.Models;

namespace CryptoWatcher.UniswapModule.Services;

public interface IUniswapPoolProvider<in TPosition> where TPosition : IUniswapPosition
{
    Task<LiquidityPool> GetPoolAsync(IWeb3 web3, UniswapNetwork uniswapNetwork, TPosition position);
}