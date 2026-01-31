using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Morpho.Application.Abstractions;
using CryptoWatcher.Shared.Entities;
using Hangfire.RecurringJobExtensions;
namespace CryptoWatcher.Infrastructure.CronJobs.Morpho;

public class SyncMorphoMarketPositionsCronJob
{
    private readonly IRepository<Wallet> _walletRepository;
    private readonly IMorphoMarketSynchronizer _morphoMarketSynchronizer;

    public SyncMorphoMarketPositionsCronJob(IRepository<Wallet> walletRepository,
        IMorphoMarketSynchronizer morphoMarketSynchronizer)
    {
        _walletRepository = walletRepository;
        _morphoMarketSynchronizer = morphoMarketSynchronizer;
    }

    [RecurringJob(CronRegistry.Every50Minutes, RecurringJobId = nameof(SyncMorphoMarketPositions))]
    public async Task SyncMorphoMarketPositions(CancellationToken ct)
    {
        var wallets = await _walletRepository.ListAsync(ct);

        var now = DateTime.UtcNow;

        var chainIds = new int[] { 130, 42161 };
        foreach (var wallet in wallets)
        {
            foreach (var chainId in chainIds)
            {
                // for now only unichain and arbitrum are supported
                await _morphoMarketSynchronizer.SynchronizeAsync(wallet.Address, chainId, now, ct);
            }
        }
    }
}