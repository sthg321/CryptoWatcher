using CryptoWatcher.Abstractions;
using CryptoWatcher.HyperliquidModule.Services;
using CryptoWatcher.Shared.Entities;
using TickerQ.Utilities.Base;

namespace CryptoWatcher.Infrastructure.Hyperliquid;

public class SyncHyperliquidPositions
{
    private readonly IHyperliquidPositionsSyncService _hyperliquidPositionsSyncService;
    private readonly IRepository<Wallet> _walletRepository;

    public SyncHyperliquidPositions(IHyperliquidPositionsSyncService hyperliquidPositionsSyncService, IRepository<Wallet> walletRepository)
    {
        _hyperliquidPositionsSyncService = hyperliquidPositionsSyncService;
        _walletRepository = walletRepository;
    }

    [TickerFunction(nameof(SyncHyperliquidPositionsAsync), "0 */1 * * *")]
    public async Task SyncHyperliquidPositionsAsync(CancellationToken ct)
    {
        var wallets = await _walletRepository.ListAsync(ct);

        var now = DateTime.Now;
        foreach (var wallet in wallets)
        {
            await _hyperliquidPositionsSyncService.SyncPositionsAsync(wallet, now, ct);
        }
    }
}