using CryptoWatcher.Modules.Uniswap.Entities;
using Microsoft.Extensions.Caching.Memory;
using Nethereum.Web3;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services;

public interface IWeb3Factory
{
    IWeb3 GetWeb3(UniswapChainConfiguration chain);
}

internal class Web3Factory : IWeb3Factory
{
    private readonly TimeSpan _web3CacheLifeTime = TimeSpan.FromMinutes(1);
    private readonly IMemoryCache _web3Cache;

    public Web3Factory(IMemoryCache web3Cache)
    {
        _web3Cache = web3Cache;
    }

    public IWeb3 GetWeb3(UniswapChainConfiguration chain)
    {
        return _web3Cache.GetOrCreate(chain, entry =>
        {
            entry.SetAbsoluteExpiration(_web3CacheLifeTime);

            return new Web3(chain.RpcUrlWithAuthToken.ToString());
        })!;
    }
}