using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Application.Abstractions;

public interface IDailyPositionPerformanceSynchronizer
{
    string Name { get; }

    Task SynchronizeAsync(IReadOnlyCollection<Wallet> wallets, DateOnly from, DateOnly to,
        CancellationToken ct = default);
}