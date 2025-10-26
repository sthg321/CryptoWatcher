using CryptoWatcher.Abstractions;
using CryptoWatcher.Application.Abstractions;
using CryptoWatcher.Shared.Entities;
using TickerQ.Utilities.Base;

namespace CryptoWatcher.Infrastructure.CronJobs;

public class SyncDailyPositionPerformanceCronJob
{
    private static int _isRunning;

    private readonly IRepository<Wallet> _walletRepository;
    private readonly IEnumerable<IDailyPositionPerformanceSynchronizer> _synchronizers;

    public SyncDailyPositionPerformanceCronJob(IRepository<Wallet> walletRepository,
        IEnumerable<IDailyPositionPerformanceSynchronizer> synchronizers)
    {
        _walletRepository = walletRepository;
        _synchronizers = synchronizers;
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
            var wallets = await _walletRepository.ListAsync(ct);

            var now = DateTime.Now;

            var nowDateOnly = DateOnly.FromDateTime(now);
            foreach (var synchronizer in _synchronizers)
            {
                await synchronizer.SynchronizeAsync(wallets, nowDateOnly, nowDateOnly, ct);
            }
        }
        finally
        {
            _isRunning = 0;
        }
    }
}