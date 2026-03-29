using CryptoWatcher.Models;
using CryptoWatcher.Modules.Infrastructure.Shared.Integrations.Abstractions;
using Nethereum.JsonRpc.Client;
using Nethereum.Web3;

namespace CryptoWatcher.Modules.Infrastructure.Shared.Integrations;

public class Web3Gateway : IWeb3Gateway
{
    private readonly BlockchainRegistry _blockchainRegistry;
    private readonly IWeb3RpcHostProvider _web3RpcHostProvider;
    private readonly IHttpClientFactory _httpClientFactory;
    
    internal const string HttpClientName = "Web3";

    public Web3Gateway(BlockchainRegistry blockchainRegistry, IWeb3RpcHostProvider web3RpcHostProvider,
        IHttpClientFactory httpClientFactory)
    {
        _blockchainRegistry = blockchainRegistry;
        _web3RpcHostProvider = web3RpcHostProvider;
        _httpClientFactory = httpClientFactory;
    }

    public IWeb3 GetConfigured(int chainId)
    {
        var chain = _blockchainRegistry.GetNetwork(chainId);

        var rpc = _web3RpcHostProvider.Create(chain.Name);

        var client = _httpClientFactory.CreateClient("Web3");
        
        return new Web3(new RpcClient(rpc, client));
    }
}