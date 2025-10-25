using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Application.Abstractions;

public interface IDailyBalanceChangeSynchronizer
{
    string Name { get; }

    Task SynchronizeAsync(IReadOnlyCollection<Wallet> wallets, DateOnly from, DateOnly to,
        CancellationToken ct = default);
}