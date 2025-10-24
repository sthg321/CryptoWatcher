namespace CryptoWatcher.Modules.Hyperliquid.Application.Abstractions;

public interface IHyperliquidBalanceChangeOrchestrator
{
    Task SynchronizeDailyBalanceChangesAsync(DateOnly from, DateOnly to, CancellationToken ct = default);
}