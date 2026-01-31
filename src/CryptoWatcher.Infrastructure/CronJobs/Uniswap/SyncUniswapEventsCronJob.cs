using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using Hangfire.RecurringJobExtensions;

namespace CryptoWatcher.Infrastructure.CronJobs.Uniswap;

public class SyncUniswapEventsCronJob
{
    private readonly IUniswapWalletPositionsSyncJob _chainSynchronizerJob;

    public SyncUniswapEventsCronJob(IUniswapWalletPositionsSyncJob chainSynchronizerJob)
    {
        _chainSynchronizerJob = chainSynchronizerJob;
    }

    [RecurringJob(CronRegistry.EveryMinute, RecurringJobId = nameof(SyncUniswapEventsAsync))]
    public async Task SyncUniswapEventsAsync()
    {
        await _chainSynchronizerJob.SynchronizeAsync();
    }
}