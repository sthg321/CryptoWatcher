using CryptoWatcher.Abstractions;
using CryptoWatcher.Shared.Entities;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Application.Abstractions;

public abstract class BaseDailyPositionPerformanceSynchronizer<TBalanceChange>
    where TBalanceChange : class
{
    private readonly IRepository<TBalanceChange> _balanceChangeRepository;
    private readonly ILogger _logger;

    protected BaseDailyPositionPerformanceSynchronizer(
        IRepository<TBalanceChange> balanceChangeRepository,
        ILogger logger)
    {
        _balanceChangeRepository = balanceChangeRepository;
        _logger = logger;
    }

    public async Task SynchronizeAsync(IReadOnlyCollection<Wallet> wallets, DateOnly from, DateOnly to,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Starting synchronization for {Protocol}", GetType().Name);

        var changes = await GetDailyBalanceChangesAsync(wallets, from, to, ct);

        await _balanceChangeRepository.BulkMergeAsync(changes, ct);

        _logger.LogInformation("Synchronization completed for {Protocol}", GetType().Name);
    }

    protected abstract Task<List<TBalanceChange>> GetDailyBalanceChangesAsync(
        IReadOnlyCollection<Wallet> wallets, DateOnly from, DateOnly to, CancellationToken ct);
}