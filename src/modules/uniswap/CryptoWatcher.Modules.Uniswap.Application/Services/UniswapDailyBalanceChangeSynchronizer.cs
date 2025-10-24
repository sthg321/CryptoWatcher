using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Services;

public class UniswapDailyBalanceChangeSynchronizer : IUniswapDailyBalanceChangeSynchronizer
{
    private readonly IRepository<Wallet> _walletRepository;
    private readonly IRepository<UniswapDailyBalanceChange> _balanceChangeRepository;
    private readonly IUniswapDailyBalanceChangeService _balanceChangeService;

    public UniswapDailyBalanceChangeSynchronizer(IRepository<Wallet> walletRepository,
        IRepository<UniswapDailyBalanceChange> balanceChangeRepository,
        IUniswapDailyBalanceChangeService balanceChangeService)
    {
        _walletRepository = walletRepository;
        _balanceChangeRepository = balanceChangeRepository;
        _balanceChangeService = balanceChangeService;
    }

    public async Task SynchronizeAsync(DateOnly from, DateOnly to, CancellationToken ct = default)
    {
        var wallets = await _walletRepository.ListAsync(ct);

        var result = await _balanceChangeService.GetDailyBalanceChangeAsync(wallets, from, to, ct);

        await _balanceChangeRepository.BulkMergeAsync(result, ct);
    }
}