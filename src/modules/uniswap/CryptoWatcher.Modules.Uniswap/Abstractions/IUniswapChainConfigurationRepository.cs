using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Abstractions;

public interface IUniswapChainConfigurationRepository
{
    Task<UniswapChainConfiguration> GetByIdAsync(int chainId, CancellationToken ct = default);
    
    Task<UniswapChainConfiguration[]> GetAllAsync(CancellationToken ct = default);
}