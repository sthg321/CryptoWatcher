using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Morpho.Application.Abstractions;
using CryptoWatcher.Shared.Entities;
using TickerQ.Utilities.Base;

namespace CryptoWatcher.Infrastructure.CronJobs.Morpho;

public class SyncMorphoMarketPositionsCronJob
{
    private readonly IRepository<Wallet> _walletRepository;
    private readonly IMorphoMarketSynchronizer  _morphoMarketSynchronizer;
    
    public SyncMorphoMarketPositionsCronJob(IRepository<Wallet> walletRepository, IMorphoMarketSynchronizer morphoMarketSynchronizer)
    {
        _walletRepository = walletRepository;
        _morphoMarketSynchronizer = morphoMarketSynchronizer;
    }

    [TickerFunction(nameof(SyncMorphoMarketPositions), CronRegistry.Every50Minutes)]
    public async Task SyncMorphoMarketPositions(CancellationToken ct)
    {
        var wallets = await _walletRepository.ListAsync(ct);

        var now = DateTime.UtcNow;
        
        foreach (var wallet in wallets)
        {
            // for now only unichain is supported
            await _morphoMarketSynchronizer.SynchronizeAsync(wallet.Address, 130, now, ct);
        }
    }
}