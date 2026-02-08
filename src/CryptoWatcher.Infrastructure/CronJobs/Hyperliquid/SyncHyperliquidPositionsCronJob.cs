using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Application.Features.Synchronization.VaultSynchronization.Abstractions;
using CryptoWatcher.Shared.Entities;
using Microsoft.Extensions.Logging;
using Hangfire.RecurringJobExtensions;
using JetBrains.Annotations;

namespace CryptoWatcher.Infrastructure.CronJobs.Hyperliquid;

[UsedImplicitly]
public class SyncHyperliquidPositionsCronJob
{
    private readonly IRepository<Wallet> _walletRepository;
    private readonly ILogger<SyncHyperliquidPositionsCronJob> _logger;
    private readonly IHyperliquidVaultPositionSyncJob _positionSyncJob;

    public SyncHyperliquidPositionsCronJob(
        IRepository<Wallet> walletRepository, ILogger<SyncHyperliquidPositionsCronJob> logger, IHyperliquidVaultPositionSyncJob positionSyncJob)
    {
        _walletRepository = walletRepository;
        _logger = logger;
        _positionSyncJob = positionSyncJob;
    }

    [RecurringJob(CronRegistry.Every50Minutes, RecurringJobId = nameof(SyncHyperliquidPositionsAsync))]
    public async Task SyncHyperliquidPositionsAsync(CancellationToken ct)
    {
        using var scope = _logger.BeginScope("Syncing {SynchronizationPlatform} positions", "Hyperliquid");

        var wallets = await _walletRepository.ListAsync(ct);

        var now = DateTime.UtcNow;

        _logger.LogInformation("Found: {WalletsCount} wallets", wallets.Count);

        foreach (var wallet in wallets)
        {
            try
            {
                _logger.LogInformation("Processing positions for wallet: {WalletAddress}", wallet.Address);

                await _positionSyncJob.SyncPositionAsync(wallet.Address, now, ct);
                
                _logger.LogInformation("Positions for wallet: {WalletAddress} processed", wallet.Address);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error syncing positions for wallet: {WalletAddress}", wallet.Address);
            }
        }
    }
}