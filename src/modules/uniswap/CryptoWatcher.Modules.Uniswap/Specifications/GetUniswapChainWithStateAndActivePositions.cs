using Ardalis.Specification;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Specifications;

public class GetUniswapChainWithStateAndActivePositions : Specification<UniswapChainConfiguration>
{
    public GetUniswapChainWithStateAndActivePositions()
    {
        Query.Include(configuration => configuration.LiquidityPoolPositions);
    }
}