using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV3.LiquidityPool;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV3.LiquidityPoolFactory;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV3.PositionsFetcher;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV3;

internal class UniswapV3Client
{
    public UniswapV3Client(IUniswapV3LiquidityPool liquidityPool, IUniswapV3PoolFactory poolFactory,
        IUniswapV3PositionFetcher positionFetcher)
    {
        LiquidityPool = liquidityPool;
        PoolFactory = poolFactory;
        PositionFetcher = positionFetcher;
    }

    public IUniswapV3LiquidityPool LiquidityPool { get; }

    public IUniswapV3PoolFactory PoolFactory { get; }

    public IUniswapV3PositionFetcher PositionFetcher { get; }
}