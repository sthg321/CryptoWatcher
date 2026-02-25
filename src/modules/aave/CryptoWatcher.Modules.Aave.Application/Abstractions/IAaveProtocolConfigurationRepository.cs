using CryptoWatcher.Modules.Aave.Entities;

namespace CryptoWatcher.Modules.Aave.Application.Abstractions;

public interface IAaveProtocolConfigurationRepository
{
    Task<IReadOnlyCollection<AaveProtocolConfiguration>> GetAaveProtocolConfigurationsAsync(
        CancellationToken ct = default);
}