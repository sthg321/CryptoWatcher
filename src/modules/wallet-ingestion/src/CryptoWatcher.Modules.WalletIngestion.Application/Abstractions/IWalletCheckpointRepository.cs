using CryptoWatcher.Modules.WalletIngestion.Entities;

namespace CryptoWatcher.Modules.WalletIngestion.Application.Abstractions;

public interface IWalletCheckpointRepository
{
    Task<WalletIngestionCheckpoint[]> GetAllAsync(CancellationToken ct = default);

    Task SaveCheckpointsAsync(WalletIngestionCheckpoint checkpoint, CancellationToken ct = default);
}