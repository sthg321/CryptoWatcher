using CryptoWatcher.Modules.Infrastructure.Shared.Integrations.Abstractions;

namespace CryptoWatcher.Modules.Infrastructure.Shared.Integrations;

public class Web3RpcHostProvider : IWeb3RpcHostProvider
{
    private readonly DrpcConfig _config;

    public Web3RpcHostProvider(DrpcConfig config)
    {
        _config = config;
    }
 
    public Uri Create(string networkName)
    {
        return new Uri($"{_config.Host}/{networkName.ToLower()}/{_config.Token}");
    }
}