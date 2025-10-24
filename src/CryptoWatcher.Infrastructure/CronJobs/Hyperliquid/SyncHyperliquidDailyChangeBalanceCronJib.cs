using CryptoWatcher.Modules.Hyperliquid.Application.Abstractions;
using TickerQ.Utilities.Base;

namespace CryptoWatcher.Infrastructure.CronJobs.Hyperliquid;

public class SyncHyperliquidDailyChangeBalanceCronJib
{
    private readonly IHyperliquidBalanceChangeOrchestrator _balanceChangeOrchestrator;

    public SyncHyperliquidDailyChangeBalanceCronJib(IHyperliquidBalanceChangeOrchestrator balanceChangeOrchestrator)
    {
        _balanceChangeOrchestrator = balanceChangeOrchestrator;
    }

    [TickerFunction(nameof(SyncDailyChangeBalanceAsync), "* * * * *")]
    public async Task SyncDailyChangeBalanceAsync()
    {
        var now = DateTime.Now;

        var nowDateOnly = DateOnly.FromDateTime(now);

        await _balanceChangeOrchestrator.SynchronizeDailyBalanceChangesAsync(nowDateOnly, nowDateOnly,
            CancellationToken.None);
    }
}