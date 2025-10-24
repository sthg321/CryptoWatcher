using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Application.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Entities;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Hyperliquid.Application.Services;

public class HyperliquidBalanceChangeOrchestrator : IHyperliquidBalanceChangeOrchestrator
{
    private readonly IRepository<HyperliquidDailyBalanceChange> _dailyBalanceChangeRepository;
    private readonly IRepository<Wallet> _walletRepository;
    private readonly IHyperliquidBalanceChangeService _balanceChangeService;

    public HyperliquidBalanceChangeOrchestrator(IRepository<HyperliquidDailyBalanceChange> dailyBalanceChangeRepository,
        IRepository<Wallet> walletRepository, IHyperliquidBalanceChangeService balanceChangeService)
    {
        _dailyBalanceChangeRepository = dailyBalanceChangeRepository;
        _walletRepository = walletRepository;
        _balanceChangeService = balanceChangeService;
    }

    public async Task SynchronizeDailyBalanceChangesAsync(DateOnly from,
        DateOnly to, CancellationToken ct = default)
    {
        var wallets = await _walletRepository.ListAsync(ct);
        
        var result = await _balanceChangeService.GetDailyBalanceChangesAsync(wallets, from, to, ct);

        await _dailyBalanceChangeRepository.BulkMergeAsync(result, ct);
    }
}