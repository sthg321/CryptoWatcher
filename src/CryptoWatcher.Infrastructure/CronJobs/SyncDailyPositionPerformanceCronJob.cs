using CryptoWatcher.Abstractions;
using CryptoWatcher.Application.Abstractions;
using CryptoWatcher.Shared.Entities;
using TickerQ.Utilities.Base;

namespace CryptoWatcher.Infrastructure.CronJobs;

public class SyncDailyPositionPerformanceCronJob
{
    private readonly IRepository<Wallet> _walletRepository;
    private readonly IEnumerable<IDailyPositionPerformanceSynchronizer> _synchronizers;

    public SyncDailyPositionPerformanceCronJob(IRepository<Wallet> walletRepository, IEnumerable<IDailyPositionPerformanceSynchronizer> synchronizers)
    {
        _walletRepository = walletRepository;
        _synchronizers = synchronizers;
    }
    
    [TickerFunction(nameof(SyncDailyBalanceChangesAsync), "* * * * *")]
    public async Task SyncDailyBalanceChangesAsync(CancellationToken ct = default)
    {
        var wallets = await _walletRepository.ListAsync(ct);

        var now = DateTime.Now;
        
        var nowDateOnly = DateOnly.FromDateTime(now);
        foreach (var synchronizer in _synchronizers)
        {
            await synchronizer.SynchronizeAsync(wallets, nowDateOnly, nowDateOnly, ct);
        }
    }
}