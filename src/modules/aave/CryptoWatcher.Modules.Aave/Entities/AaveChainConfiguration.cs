using CryptoWatcher.Modules.Aave.ValueObjects;

namespace CryptoWatcher.Modules.Aave.Entities;

public class AaveChainConfiguration
{
    public string Name { get; init; } = null!;

    public Uri RpcUrl { get; init; } = null!;

    public string? RpcAuthToken { get; init; }

    public Uri RpcUrlWithAuthToken => RpcAuthToken is not null ? new Uri($"{RpcUrl}/{RpcAuthToken}") : RpcUrl;

    public AaveAddresses SmartContractAddresses { get; init; } = null!;
}