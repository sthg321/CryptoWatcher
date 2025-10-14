using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Specifications;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Modules.Uniswap.Application.Services;

public class UniswapChainSynchronizationOrchestrator : IUniswapChainSynchronizerOrchestrator
{
    private static int _isRunning;

    private readonly IUniswapChainSynchronizer _chainSynchronizer;
    private readonly IRepository<UniswapChainConfiguration> _chainConfigurationRepository;
    private readonly ILogger<UniswapChainSynchronizationOrchestrator> _logger;

    public UniswapChainSynchronizationOrchestrator(IUniswapChainSynchronizer chainSynchronizer,
        IRepository<UniswapChainConfiguration> chainConfigurationRepository,
        ILogger<UniswapChainSynchronizationOrchestrator> logger)
    {
        _chainSynchronizer = chainSynchronizer;
        _chainConfigurationRepository = chainConfigurationRepository;
        _logger = logger;
    }

    public async Task SynchronizeAllChainsAsync(CancellationToken ct = default)
    {
        if (Interlocked.CompareExchange(ref _isRunning, 1, 0) == 1)
        {
            _logger.LogWarning("Synchronization is already in progress");
            return;
        }

        try
        {
            _isRunning = 1;

            var chainsToSynchronize =
                await _chainConfigurationRepository.ListAsync(new GetUniswapChainWithStateAndActivePositions(), ct);

            foreach (var uniswapChainConfiguration in chainsToSynchronize)
            {
                try
                {
                    await _chainSynchronizer.SynchronizeChainAsync(uniswapChainConfiguration, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occured while synchronizing the chain");
                }
            }
        }
        finally
        {
            _isRunning = 0;
        }
    }
}