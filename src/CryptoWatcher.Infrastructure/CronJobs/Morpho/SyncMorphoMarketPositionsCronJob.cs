using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Morpho.Application.Abstractions;
using CryptoWatcher.Shared.Entities;
using Hangfire.RecurringJobExtensions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Infrastructure.CronJobs.Morpho;

[UsedImplicitly]
public class SyncMorphoMarketPositionsCronJob
{
    private readonly IRepository<Wallet> _walletRepository;
    private readonly IMorphoMarketSynchronizer _morphoMarketSynchronizer;
    private readonly ILogger<SyncMorphoMarketPositionsCronJob> _logger;

    public SyncMorphoMarketPositionsCronJob(IRepository<Wallet> walletRepository,
        IMorphoMarketSynchronizer morphoMarketSynchronizer, ILogger<SyncMorphoMarketPositionsCronJob> logger)
    {
        _walletRepository = walletRepository;
        _morphoMarketSynchronizer = morphoMarketSynchronizer;
        _logger = logger;
    }

    [RecurringJob(CronRegistry.Every50Minutes, RecurringJobId = nameof(SyncMorphoMarketPositions))]
    public async Task SyncMorphoMarketPositions(CancellationToken ct)
    {
        var wallets = await _walletRepository.ListAsync(ct);

        var now = DateTime.UtcNow;

        var chainIds = new[] { 130, 42161 };
        foreach (var wallet in wallets)
        {
            foreach (var chainId in chainIds)
            {
                // for now only unichain and arbitrum are supported
                try
                {
                    await _morphoMarketSynchronizer.SynchronizeAsync(wallet.Address, chainId, now, ct);
                }
                catch (Exception e)
                {
                    _logger.LogError(e,
                        "Unable to sync morpho market positions for wallet: {WalletAddress} and chainId: {ChainId}",
                        wallet.Address, chainId);
                }
            }
        }
    }
}