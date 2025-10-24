namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IUniswapDailyBalanceChangeSynchronizer
{
    Task SynchronizeAsync(DateOnly from, DateOnly to, CancellationToken ct = default);
}