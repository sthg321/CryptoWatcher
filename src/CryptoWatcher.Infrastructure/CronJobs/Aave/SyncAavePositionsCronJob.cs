using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Aave.Application.Abstractions;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.Shared.Entities;
using Microsoft.Extensions.Logging;
using Hangfire.RecurringJobExtensions;
using JetBrains.Annotations;

namespace CryptoWatcher.Infrastructure.CronJobs.Aave;

[UsedImplicitly]
public class SyncAavePositionsCronJob
{
    private readonly IAavePositionsSyncService _positionsSyncService;
    private readonly IRepository<Wallet> _walletRepository;
    private readonly IAaveProtocolConfigurationRepository _aaveNetworkRepository;
    private readonly ILogger<SyncAavePositionsCronJob> _logger;

    public SyncAavePositionsCronJob(IAavePositionsSyncService positionsSyncService,
        IRepository<Wallet> walletRepository, IAaveProtocolConfigurationRepository aaveNetworkRepository,
        ILogger<SyncAavePositionsCronJob> logger)
    {
        _positionsSyncService = positionsSyncService;
        _walletRepository = walletRepository;
        _logger = logger;
        _aaveNetworkRepository = aaveNetworkRepository;
    }

    [RecurringJob(CronRegistry.Every50Minutes, RecurringJobId = nameof(SyncAavePositionsAsync))]
    public async Task SyncAavePositionsAsync(CancellationToken ct = default)
    {
        _logger.SynchronizationStarted();

        var wallets = await _walletRepository.ListAsync(ct);

        var chains = await _aaveNetworkRepository.GetAaveProtocolConfigurationsAsync(ct);

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
                    _logger.LogError(e, "Unable to sync aave chain. ChainName: {ChainName}", chainConfiguration.Name);
                }
            }
        }

        _logger.SynchronizationCompleted();
    }
}