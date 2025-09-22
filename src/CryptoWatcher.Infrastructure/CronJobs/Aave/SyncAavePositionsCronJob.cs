using CryptoWatcher.AaveModule.Models;
using CryptoWatcher.AaveModule.Services;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Shared.Entities;
using Microsoft.Extensions.Logging;
using TickerQ.Utilities.Base;

namespace CryptoWatcher.Infrastructure.CronJobs.Aave;

internal class SyncAavePositionsCronJob
{
    private readonly IAavePositionsSyncService _positionsSyncService;
    private readonly IRepository<Wallet> _walletRepository;
    private readonly ILogger<SyncAavePositionsCronJob> _logger;

    public SyncAavePositionsCronJob(IAavePositionsSyncService positionsSyncService,
        IRepository<Wallet> walletRepository, ILogger<SyncAavePositionsCronJob> logger)
    {
        _positionsSyncService = positionsSyncService;
        _walletRepository = walletRepository;
        _logger = logger;
    }

    [TickerFunction(nameof(SyncAavePositionsAsync), CronRegistry.Every50Minutes)]
    public async Task SyncAavePositionsAsync(CancellationToken ct = default)
    {
        _logger.SynchronizationStarted();

        var wallets = await _walletRepository.ListAsync(ct);

        _logger.WalletsFound(wallets.Count);

        var now = DateOnly.FromDateTime(DateTime.Now);
        foreach (var wallet in wallets)
        {
            foreach (var aaveNetwork in AaveNetwork.All)
            {
                await _positionsSyncService.SyncPositionsAsync(aaveNetwork, wallet, now, ct);
            }
        }

        _logger.SynchronizationCompleted();
    }
}