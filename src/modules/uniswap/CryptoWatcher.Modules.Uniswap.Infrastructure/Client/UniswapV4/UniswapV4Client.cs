using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV4.LiquidityPool;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV4.PositionsFetcher;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV4;

internal class UniswapV4Client
{
    public UniswapV4Client(IUniswapV4LiquidityPool liquidityPool, IUniswapV4PositionFetcher positionFetcher)
    {
        LiquidityPool = liquidityPool;
        PositionFetcher = positionFetcher;
    }

    public IUniswapV4LiquidityPool LiquidityPool { get; }

    public IUniswapV4PositionFetcher PositionFetcher { get; }
}