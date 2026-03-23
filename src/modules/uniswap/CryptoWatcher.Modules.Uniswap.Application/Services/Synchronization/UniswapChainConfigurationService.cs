using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization;

public class UniswapChainConfigurationService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IUniswapChainConfigurationRepository _chainConfigurationRepository;

    public UniswapChainConfigurationService(IMemoryCache memoryCache,
        IUniswapChainConfigurationRepository chainConfigurationRepository)
    {
        _memoryCache = memoryCache;
        _chainConfigurationRepository = chainConfigurationRepository;
    }

    public async ValueTask<UniswapChainConfiguration> GetByIdAsync(int chainId, CancellationToken ct)
    {
        var result = await _memoryCache.GetOrCreateAsync(chainId, async _ =>
        {
            var chain = await _chainConfigurationRepository.GetByIdAsync(chainId, ct);

            return chain;
        });

        return result ?? throw new InvalidOperationException(
            $"Chain configuration for chainId={chainId} not found");
    }
}