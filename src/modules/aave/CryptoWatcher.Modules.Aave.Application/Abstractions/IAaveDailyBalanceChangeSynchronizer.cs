namespace CryptoWatcher.Modules.Aave.Application.Abstractions;

public interface IAaveDailyBalanceChangeSynchronizer
{
    Task SynchronizeAsync(DateOnly from, DateOnly to, CancellationToken ct = default);
}