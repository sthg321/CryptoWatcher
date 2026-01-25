using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using Microsoft.Extensions.Logging;
using TickerQ.Utilities.Base;

namespace CryptoWatcher.Infrastructure.CronJobs.Uniswap;

internal class SyncUniswapPoolPositionsCronJob
{
    private readonly IUniswapWalletSyncOrchestrator _syncOrchestrator;
    private readonly ILogger<SyncUniswapPoolPositionsCronJob> _logger;

    public SyncUniswapPoolPositionsCronJob(IUniswapWalletSyncOrchestrator syncOrchestrator,
        ILogger<SyncUniswapPoolPositionsCronJob> logger)
    {
        _syncOrchestrator = syncOrchestrator;
        _logger = logger;
    }

    [TickerFunction(nameof(SyncUniswapPoolPositionsAsync), CronRegistry.Every50Minutes)]
    public async Task SyncUniswapPoolPositionsAsync(CancellationToken ct)
    {
        _logger.StartingPoolSync();

        await _syncOrchestrator.SyncWalletPositionsAsync(ct);

        _logger.PoolSyncCompleted();
    }
}