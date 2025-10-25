using TickerQ.Utilities.Base;

namespace CryptoWatcher.Infrastructure.CronJobs.Hyperliquid;

public class SyncHyperliquidDailyChangeBalanceCronJib
{
    
    [TickerFunction(nameof(SyncDailyChangeBalanceAsync), "* * * * *")]
    public async Task SyncDailyChangeBalanceAsync()
    {
        var now = DateTime.Now;

        var nowDateOnly = DateOnly.FromDateTime(now);

        
    }
}