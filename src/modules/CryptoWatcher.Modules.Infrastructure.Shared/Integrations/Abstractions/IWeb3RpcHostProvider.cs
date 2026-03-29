namespace CryptoWatcher.Modules.Infrastructure.Shared.Integrations.Abstractions;

public interface IWeb3RpcHostProvider
{
    Uri Create(string networkName);
}