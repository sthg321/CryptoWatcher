using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IUniswapDailyBalanceChangeService
{
    Task<List<UniswapDailyBalanceChange>> GetDailyBalanceChangeAsync(IReadOnlyCollection<Wallet> wallets,
        DateOnly from, DateOnly to,
        CancellationToken ct = default);
}