using CryptoWatcher.Entities;
using CryptoWatcher.Entities.Uniswap;
using CryptoWatcher.Models;
using UniswapClient.Models;

namespace CryptoWatcher.Integrations;

public interface IUniswapProvider
{
    Task<List<IUniswapPosition>> GetPositionsAsync(UniswapNetwork uniswapNetwork, Wallet wallet);
    
    Task<LiquidityPool> GetPoolAsync(UniswapNetwork uniswapNetwork, IUniswapPosition position);

    PositionInPool GetPoolPositionAsync(LiquidityPool pool, IUniswapPosition position);

    TokenPair GetPositionFee(LiquidityPool pool, IUniswapPosition position);
}