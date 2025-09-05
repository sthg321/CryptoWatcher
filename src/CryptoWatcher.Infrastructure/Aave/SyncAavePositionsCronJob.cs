using CryptoWatcher.AaveModule.Services;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Shared.Entities;
using Microsoft.Extensions.Logging;
using TickerQ.Utilities.Base;

namespace CryptoWatcher.Infrastructure.Aave;

public class SyncAavePositionsCronJob
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

    [TickerFunction(nameof(SyncPositionsAsync), "0 */1 * * *")]
    public async Task SyncPositionsAsync(CancellationToken ct = default)
    {
        _logger.SynchronizationStarted();
        
        var wallets = await _walletRepository.ListAsync(ct);
        
        _logger.WalletsFound(wallets.Count);
        
        var now = DateOnly.FromDateTime(DateTime.Now);
        foreach (var wallet in wallets)
        {
            await _positionsSyncService.SyncPositionsAsync(wallet, now, ct);
        }

        _logger.SynchronizationCompleted();
    }
}