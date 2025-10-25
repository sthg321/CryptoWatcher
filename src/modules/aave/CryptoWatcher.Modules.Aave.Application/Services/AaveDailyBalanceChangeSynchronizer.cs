using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Aave.Application.Abstractions;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Aave.Application.Services;

public class AaveDailyBalanceChangeSynchronizer : IAaveDailyBalanceChangeSynchronizer
{
    private readonly IRepository<Wallet> _walletRepository;

    public AaveDailyBalanceChangeSynchronizer(IRepository<Wallet> walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public Task SynchronizeAsync(DateOnly from, DateOnly to, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}