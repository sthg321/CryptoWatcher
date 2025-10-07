using UniswapClient.UniswapV4.LiquidityPool;
using UniswapClient.UniswapV4.PositionsFetcher;

namespace UniswapClient.UniswapV4;

public class UniswapV4Client
{
    public UniswapV4Client(IUniswapV4LiquidityPool liquidityPool, IUniswapV4PositionFetcher positionFetcher)
    {
        LiquidityPool = liquidityPool;
        PositionFetcher = positionFetcher;
    }

    public IUniswapV4LiquidityPool LiquidityPool { get; }

    public IUniswapV4PositionFetcher PositionFetcher { get; }
}