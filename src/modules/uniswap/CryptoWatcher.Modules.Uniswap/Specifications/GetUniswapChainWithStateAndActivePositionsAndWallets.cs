using Ardalis.Specification;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Specifications;

public class GetUniswapChainWithStateAndActivePositionsAndWallets : Specification<UniswapChainConfiguration>
{
    public GetUniswapChainWithStateAndActivePositionsAndWallets()
    {
        Query.Include(configuration => configuration.LiquidityPoolPositions)
            .ThenInclude(positions => positions.Wallet);
    }
}