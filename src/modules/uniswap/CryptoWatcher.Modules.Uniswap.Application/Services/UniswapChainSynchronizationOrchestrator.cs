using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Specifications;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Modules.Uniswap.Application.Services;

public class UniswapChainSynchronizationOrchestrator : IUniswapChainSynchronizerOrchestrator
{
    private static int _isRunning;

    private readonly IRepository<UniswapChainConfiguration> _chainConfigurationRepository;
    private readonly IBlockscoutTransactionSynchronizer _blockscoutTransactionSynchronizer;
    private readonly ILogger<UniswapChainSynchronizationOrchestrator> _logger;

    public UniswapChainSynchronizationOrchestrator(IRepository<UniswapChainConfiguration> chainConfigurationRepository,
        IBlockscoutTransactionSynchronizer blockscoutTransactionSynchronizer,
        ILogger<UniswapChainSynchronizationOrchestrator> logger)
    {
        _chainConfigurationRepository = chainConfigurationRepository;
        _blockscoutTransactionSynchronizer = blockscoutTransactionSynchronizer;
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
                await _chainConfigurationRepository.ListAsync(new GetUniswapChainWithStateAndActivePositionsAndWallets(), ct);

            foreach (var uniswapChainConfiguration in chainsToSynchronize)
            {
                _logger.LogInformation("Begin synchronization for uniswap chain: {UniswapChain}",
                    uniswapChainConfiguration.Name);

                try
                {
                    foreach (var position in uniswapChainConfiguration.LiquidityPoolPositions.GroupBy(position =>
                                 position.Wallet))
                    {
                        await _blockscoutTransactionSynchronizer.SyncAsync(uniswapChainConfiguration,
                            position.Key,
                            ct);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occured while synchronizing the chain");
                }
                
                _logger.LogInformation("Finished synchronization for uniswap chain: {UniswapChain}",
                    uniswapChainConfiguration.Name);
            }
        }
        finally
        {
            _isRunning = 0;
        }
    }
}