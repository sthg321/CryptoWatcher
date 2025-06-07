using CryptoWatcher.Entities;
using CryptoWatcher.Models;
using UniswapClient.Models;

namespace CryptoWatcher.Integrations;

public interface IUniswapProvider
{
    Task<LiquidityPool> GetPoolAsync(Network network, IUniswapPosition uniswapPosition);

    Task<PositionInPool> GetPoolPositionAsync(LiquidityPool pool);

    Task<TokenPair> GetPoolFeeAsync(LiquidityPool pool);
}