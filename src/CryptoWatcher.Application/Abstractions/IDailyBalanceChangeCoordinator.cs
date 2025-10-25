namespace CryptoWatcher.Application.Abstractions;

public interface IDailyBalanceChangeCoordinator
{
    Task SynchronizeDailyBalanceChangesAsync(DateOnly from, DateOnly to, CancellationToken ct = default);
}