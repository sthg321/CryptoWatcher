using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Merkl.Application.Abstractions;
using CryptoWatcher.Shared.Entities;
using Microsoft.Extensions.Logging;
using TickerQ.Utilities.Base;

namespace CryptoWatcher.Infrastructure.CronJobs.Merkl;

public class SyncMerklRewardsCronJob
{
    private static int _isRunning;
    
    private readonly IRepository<Wallet> _walletRepo;
    private readonly IMerklSyncService _syncService;
    private readonly ILogger<SyncMerklRewardsCronJob> _logger;

    public SyncMerklRewardsCronJob(IRepository<Wallet> walletRepo, IMerklSyncService syncService, ILogger<SyncMerklRewardsCronJob> logger)
    {
        _walletRepo = walletRepo;
        _syncService = syncService;
        _logger = logger;
    }

    [TickerFunction(nameof(SyncMerklRewardsAsync), CronRegistry.Every50Minutes)]
    public async Task SyncMerklRewardsAsync(CancellationToken ct)
    {
        if (Interlocked.CompareExchange(ref _isRunning, 1, 0) == 1)
        {
            return;
        }
        
        var wallets = await _walletRepo.ListAsync(ct);

        var now = DateOnly.FromDateTime(DateTime.Now);
        foreach (var wallet in wallets)
        {
            try
            {
                await _syncService.SyncRewardsAsync(wallet.Address, 143, now, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on sync merkl rewards for wallet: {Wallet}", wallet.Address);
            }
        }

        _isRunning = 0;
    }
}