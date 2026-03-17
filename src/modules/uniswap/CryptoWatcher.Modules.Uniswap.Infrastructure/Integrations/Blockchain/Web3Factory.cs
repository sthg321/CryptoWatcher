using CryptoWatcher.Modules.Uniswap.Entities;
using Microsoft.Extensions.Caching.Memory;
using Nethereum.JsonRpc.Client;
using Nethereum.Web3;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Blockchain;

public interface IWeb3Factory
{
    IWeb3 GetWeb3(UniswapChainConfiguration chain);
}

internal class Web3Factory : IWeb3Factory
{
    private readonly TimeSpan _web3CacheLifeTime = TimeSpan.FromMinutes(1);
    private readonly IMemoryCache _web3Cache;
    private readonly IHttpClientFactory _clientFactory;

    public Web3Factory(IMemoryCache web3Cache, IHttpClientFactory clientFactory)
    {
        _web3Cache = web3Cache;
        _clientFactory = clientFactory;
    }

    public IWeb3 GetWeb3(UniswapChainConfiguration chain)
    {
        return _web3Cache.GetOrCreate(chain, entry =>
        {
            entry.SetAbsoluteExpiration(_web3CacheLifeTime);

            var client = _clientFactory.CreateClient("Web3");
            return new Web3(new RpcClient(chain.RpcUrlWithAuthToken, client));
        })!;
    }
}