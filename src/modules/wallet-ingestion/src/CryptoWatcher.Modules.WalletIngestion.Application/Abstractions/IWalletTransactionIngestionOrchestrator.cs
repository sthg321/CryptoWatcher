namespace CryptoWatcher.Modules.WalletIngestion.Application.Abstractions;

public interface IWalletTransactionIngestionOrchestrator
{
    /// <summary>
    /// Запускает синхронизацию транзакций пользователя.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task StartSynchronizationAsync(CancellationToken ct = default);
}