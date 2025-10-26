namespace CryptoWatcher.Application.Abstractions;

public interface IDailyPositionPerformanceCoordinator
{
    Task SynchronizeDailyBalanceChangesAsync(DateOnly from, DateOnly to, CancellationToken ct = default);
}