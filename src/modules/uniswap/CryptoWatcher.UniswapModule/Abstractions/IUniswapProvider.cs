using CryptoWatcher.Shared.Entities;
using CryptoWatcher.UniswapModule.Entities;
using CryptoWatcher.UniswapModule.Models;
using UniswapClient.Models;

namespace CryptoWatcher.UniswapModule.Services;

public interface IUniswapProvider
{
    Task<List<IUniswapPosition>> GetPositionsAsync(UniswapNetwork uniswapNetwork, Wallet wallet);
    
    Task<LiquidityPool> GetPoolAsync(UniswapNetwork uniswapNetwork, IUniswapPosition position);

    PositionInPool GetPoolPositionAsync(LiquidityPool pool, IUniswapPosition position);

    TokenPair GetPositionFee(LiquidityPool pool, IUniswapPosition position);
}