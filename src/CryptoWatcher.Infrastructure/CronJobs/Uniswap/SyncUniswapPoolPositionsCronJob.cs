using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using Hangfire.RecurringJobExtensions;

namespace CryptoWatcher.Infrastructure.CronJobs.Uniswap;

public class SyncUniswapPoolPositionsCronJob
{
    private readonly IPositionPriceSynchronizationJob _positionsSyncService;

    public SyncUniswapPoolPositionsCronJob(IPositionPriceSynchronizationJob positionsSyncService)
    {
        _positionsSyncService = positionsSyncService;
    }
    
    [RecurringJob(CronRegistry.Every50Minutes, RecurringJobId = nameof(SyncUniswapPoolPositionsAsync))]
    public async Task SyncUniswapPoolPositionsAsync(CancellationToken ct)
    {
        await _positionsSyncService.SynchronizeAsync(ct);
    }
}