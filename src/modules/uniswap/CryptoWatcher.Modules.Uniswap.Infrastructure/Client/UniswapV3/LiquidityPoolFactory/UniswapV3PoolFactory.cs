using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV3.LiquidityPoolFactory.Contracts;
using Nethereum.Web3;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV3.LiquidityPoolFactory;

internal interface IUniswapV3PoolFactory
{
    Task<string> GetPoolAddressAsync(IWeb3 web3, string poolFactoryAddress, string token0, string token1);
}

internal class UniswapV3PoolFactory : IUniswapV3PoolFactory
{
    public async Task<string> GetPoolAddressAsync(IWeb3 web3, string poolFactoryAddress, string token0, string token1)
    {
        var contract = web3.Eth.GetContract(PoolFactoryAbi.Abi, poolFactoryAddress);
        var function = contract.GetFunction("getPairPools");

        var result = await function.CallDeserializingToObjectAsync<GetPairPoolsOutputDto>(token0, token1);
        return result.Pools.First().Pool;
    }
}