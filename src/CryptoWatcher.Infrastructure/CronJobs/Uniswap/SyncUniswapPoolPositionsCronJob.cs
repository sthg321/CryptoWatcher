using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using TickerQ.Utilities.Base;

namespace CryptoWatcher.Infrastructure.CronJobs.Uniswap;

internal class SyncUniswapPoolPositionsCronJob
{
    private readonly IPositionPriceSynchronizationJob _positionsSyncService;

    public SyncUniswapPoolPositionsCronJob(IPositionPriceSynchronizationJob positionsSyncService)
    {
        _positionsSyncService = positionsSyncService;
    }

    [TickerFunction(nameof(SyncUniswapPoolPositionsAsync), CronRegistry.Every50Minutes)]
    public async Task SyncUniswapPoolPositionsAsync(CancellationToken ct)
    {
        await _positionsSyncService.SynchronizeAsync(ct);
    }
}