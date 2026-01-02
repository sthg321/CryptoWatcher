using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Aave.Application.Abstractions;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.Modules.Aave.Models;
using CryptoWatcher.Shared.Entities;
using Microsoft.Extensions.Logging;
using TickerQ.Utilities.Base;

namespace CryptoWatcher.Infrastructure.CronJobs.Aave;

internal class SyncAavePositionsCronJob
{
    private readonly IAavePositionsSyncService _positionsSyncService;
    private readonly IRepository<Wallet> _walletRepository;
    private readonly IRepository<AaveChainConfiguration> _aaveNetworkRepository;
    private readonly ILogger<SyncAavePositionsCronJob> _logger;

    public SyncAavePositionsCronJob(IAavePositionsSyncService positionsSyncService,
        IRepository<Wallet> walletRepository, ILogger<SyncAavePositionsCronJob> logger, IRepository<AaveChainConfiguration> aaveNetworkRepository)
    {
        _positionsSyncService = positionsSyncService;
        _walletRepository = walletRepository;
        _logger = logger;
        _aaveNetworkRepository = aaveNetworkRepository;
    }

    [TickerFunction(nameof(SyncAavePositionsAsync), CronRegistry.Every50Minutes)]
    public async Task SyncAavePositionsAsync(CancellationToken ct = default)
    {
        _logger.SynchronizationStarted();

        var wallets = await _walletRepository.ListAsync(ct);

        var chains = await _aaveNetworkRepository.ListAsync(ct);

        _logger.WalletsFound(wallets.Count);

        var now = DateOnly.FromDateTime(DateTime.Now);
        foreach (var wallet in wallets)
        {
            foreach (var chainConfiguration in chains)
            {
                try
                {
                    await _positionsSyncService.SyncPositionsAsync(chainConfiguration, wallet, now, ct);
                }
                catch (Exception e)
                {
                  
                }
            }
        }

        _logger.SynchronizationCompleted();
    }
}