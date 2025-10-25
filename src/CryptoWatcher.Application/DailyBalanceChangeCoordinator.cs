using CryptoWatcher.Abstractions;
using CryptoWatcher.Application.Abstractions;
using CryptoWatcher.Shared.Entities;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Application;

public class DailyBalanceChangeCoordinator : IDailyBalanceChangeCoordinator
{
    private readonly IRepository<Wallet> _walletRepository;
    private readonly IEnumerable<IDailyBalanceChangeSynchronizer> _synchronizers;
    private readonly ILogger<DailyBalanceChangeCoordinator> _logger;

    public DailyBalanceChangeCoordinator(IRepository<Wallet> walletRepository,
        IEnumerable<IDailyBalanceChangeSynchronizer> synchronizers,
        ILogger<DailyBalanceChangeCoordinator> logger)
    {
        _walletRepository = walletRepository;
        _synchronizers = synchronizers;
        _logger = logger;
    }

    public async Task SynchronizeDailyBalanceChangesAsync(DateOnly from, DateOnly to, CancellationToken ct = default)
    {
        var wallets = await _walletRepository.ListAsync(ct);

        await _walletRepository.UnitOfWork.BeginTransactionAsync(ct);

        foreach (var synchronizer in _synchronizers)
        {
            using var scope = _logger.BeginScope("SynchronizerName: {SynchronizerName}", synchronizer.Name);
            try
            {
                _logger.LogInformation("Synchronizing daily balance changes.");

                await synchronizer.SynchronizeAsync(wallets, from, to, ct);

                _logger.LogInformation("Daily balance changes");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while synchronizing daily balance changes");

                await _walletRepository.UnitOfWork.RollbackTransactionAsync(ct);
                throw;
            }
        }
    }
}