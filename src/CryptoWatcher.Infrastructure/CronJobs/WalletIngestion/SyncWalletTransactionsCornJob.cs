using CryptoWatcher.Modules.WalletIngestion.Application.Abstractions;
using JetBrains.Annotations;
using Hangfire.RecurringJobExtensions;

namespace CryptoWatcher.Infrastructure.CronJobs.WalletIngestion;

[UsedImplicitly]
public class SyncWalletTransactionsCornJob
{
    private readonly IWalletTransactionIngestionOrchestrator _walletTransactionOrchestrator;

    public SyncWalletTransactionsCornJob(IWalletTransactionIngestionOrchestrator walletTransactionOrchestrator)
    {
        _walletTransactionOrchestrator = walletTransactionOrchestrator;
    }

    [RecurringJob(CronRegistry.Every50Minutes, RecurringJobId = nameof(SyncWalletTransactionsAsync))]
    public async Task SyncWalletTransactionsAsync()
    {
        await _walletTransactionOrchestrator.StartSynchronizationAsync();
    }
}