using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using TickerQ.Utilities.Base;

namespace CryptoWatcher.Infrastructure.CronJobs.Uniswap;

public class SyncUniswapDailyChangeBalanceCronJob
{
    private readonly IUniswapDailyBalanceChangeSynchronizer _balanceChangeSynchronizer;

    public SyncUniswapDailyChangeBalanceCronJob(IUniswapDailyBalanceChangeSynchronizer balanceChangeSynchronizer)
    {
        _balanceChangeSynchronizer = balanceChangeSynchronizer;
    }

    [TickerFunction(nameof(SyncUniswapDailyChangeBalanceAsync), "* * * * *")]
    public async Task SyncUniswapDailyChangeBalanceAsync()
    {
        var now = DateTime.Now;

        var nowDateOnly = DateOnly.FromDateTime(now);

        await _balanceChangeSynchronizer.SynchronizeAsync(nowDateOnly, nowDateOnly);
    }
}