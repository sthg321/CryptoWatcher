using CryptoWatcher.Modules.Aave.ValueObjects;

namespace CryptoWatcher.Modules.Aave.Entities;

public class AaveProtocolConfiguration : BaseChainConfiguration
{
    public AaveAddresses SmartContractAddresses { get; init; } = null!;
}