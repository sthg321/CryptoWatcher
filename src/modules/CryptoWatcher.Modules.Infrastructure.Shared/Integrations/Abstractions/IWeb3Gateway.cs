using Nethereum.Web3;

namespace CryptoWatcher.Modules.Infrastructure.Shared.Integrations.Abstractions;

public interface IWeb3Gateway
{
    IWeb3 GetConfigured(int chainId);
}