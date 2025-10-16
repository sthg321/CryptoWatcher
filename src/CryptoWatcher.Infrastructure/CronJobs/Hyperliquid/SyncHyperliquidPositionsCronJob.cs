using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Application.Services;
using CryptoWatcher.Shared.Entities;
using Microsoft.Extensions.Logging;
using TickerQ.Utilities.Base;

namespace CryptoWatcher.Infrastructure.CronJobs.Hyperliquid;

public class SyncHyperliquidPositionsCronJob
{
    private readonly IHyperliquidPositionsSyncService _hyperliquidPositionsSyncService;
    private readonly IRepository<Wallet> _walletRepository;
    private readonly ILogger<SyncHyperliquidPositionsCronJob> _logger;

    public SyncHyperliquidPositionsCronJob(IHyperliquidPositionsSyncService hyperliquidPositionsSyncService,
        IRepository<Wallet> walletRepository, ILogger<SyncHyperliquidPositionsCronJob> logger)
    {
        _hyperliquidPositionsSyncService = hyperliquidPositionsSyncService;
        _walletRepository = walletRepository;
        _logger = logger;
    }

    [TickerFunction(nameof(SyncHyperliquidPositionsAsync), "0,50 * * * *")]
    public async Task SyncHyperliquidPositionsAsync(CancellationToken ct)
    {
        using var scope = _logger.BeginScope("Syncing {SynchronizationPlatform} positions", "Hyperliquid");

        var wallets = await _walletRepository.ListAsync(ct);

        var now = DateTime.Now;

        _logger.LogInformation("Found: {WalletsCount} wallets", wallets.Count);

        foreach (var wallet in wallets)
        {
            try
            {
                _logger.LogInformation("Processing positions for wallet: {WalletAddress}", wallet.Address);

                await _hyperliquidPositionsSyncService.SyncPositionsAsync(wallet, now, ct);

                _logger.LogInformation("Positions for wallet: {WalletAddress} processed", wallet.Address);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error syncing positions for wallet: {WalletAddress}", wallet.Address);
            }
        }
    }
}