using CryptoWatcher.Modules.WalletIngestion.Entities;

namespace CryptoWatcher.Modules.WalletIngestion.Application.Abstractions;

public interface IWalletTransactionIngestionService
{
    Task IngestAllAsync(WalletIngestionCheckpoint checkpoint, CancellationToken ct = default);
}