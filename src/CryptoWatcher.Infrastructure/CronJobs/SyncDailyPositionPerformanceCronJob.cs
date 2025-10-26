using CryptoWatcher.Application.Abstractions;
using TickerQ.Utilities.Base;

namespace CryptoWatcher.Infrastructure.CronJobs;

public class SyncDailyPositionPerformanceCronJob
{
    private static int _isRunning;

    private readonly IDailyPositionPerformanceCoordinator _dailyPositionPerformanceCoordinator;

    public SyncDailyPositionPerformanceCronJob(IDailyPositionPerformanceCoordinator dailyPositionPerformanceCoordinator)
    {
        _dailyPositionPerformanceCoordinator = dailyPositionPerformanceCoordinator;
    }
    
    [TickerFunction(nameof(SyncDailyPositionPerformanceAsync), "* * * * *")]
    public async Task SyncDailyPositionPerformanceAsync(CancellationToken ct = default)
    {
        if (Interlocked.CompareExchange(ref _isRunning, 1, 0) == 1)
        {
            return;
        }

        try
        {
            var now = DateTime.Now;

            var nowDateOnly = DateOnly.FromDateTime(now);
            await _dailyPositionPerformanceCoordinator.SynchronizeDailyBalanceChangesAsync(nowDateOnly, nowDateOnly,
                ct);
        }
        finally
        {
            _isRunning = 0;
        }
    }
}