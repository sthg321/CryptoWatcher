using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using TickerQ.Utilities.Base;

namespace CryptoWatcher.Infrastructure.CronJobs.Uniswap;

public class SyncUniswapEventsCronJob
{
    private readonly IUniswapChainSynchronizerOrchestrator _chainSynchronizerOrchestrator;

    public SyncUniswapEventsCronJob(IUniswapChainSynchronizerOrchestrator chainSynchronizerOrchestrator)
    {
        _chainSynchronizerOrchestrator = chainSynchronizerOrchestrator;
    }

    [TickerFunction(nameof(SyncUniswapEventsAsync), "* * * * *")]
    public async Task SyncUniswapEventsAsync()
    {
       // await _chainSynchronizerOrchestrator.SynchronizeAllChainsAsync();
    }
}