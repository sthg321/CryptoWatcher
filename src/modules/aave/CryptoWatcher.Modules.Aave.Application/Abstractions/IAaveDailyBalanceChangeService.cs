using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Aave.Application.Abstractions;

public interface IAaveDailyBalanceChangeService
{
    Task<List<AaveDailyBalanceChange>> GetDailyBalanceChangeAsync(IReadOnlyCollection<Wallet> wallets,
        DateOnly from, DateOnly to,
        CancellationToken ct = default);
}