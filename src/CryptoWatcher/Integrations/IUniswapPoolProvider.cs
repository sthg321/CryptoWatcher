using CryptoWatcher.Entities;
using CryptoWatcher.Models;
using Nethereum.Web3;
using UniswapClient.Models;

namespace CryptoWatcher.Integrations;

public interface IUniswapPoolProvider<in TPosition> where TPosition : IUniswapPosition
{
    Task<LiquidityPool> GetPoolAsync(IWeb3 web3, Network network, TPosition position);
}