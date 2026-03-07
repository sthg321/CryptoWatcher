using CryptoWatcher.Modules.WalletIngestion.Application.Abstractions;
using CryptoWatcher.Modules.WalletIngestion.Entities;
using Microsoft.EntityFrameworkCore;

namespace CryptoWatcher.Modules.WalletIngestion.Infrastructure.Persistence.Repositories;

public class WalletCheckpointRepository : IWalletCheckpointRepository
{
    private readonly WalletIngestionDbContext _dbContext;

    public WalletCheckpointRepository(WalletIngestionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<WalletIngestionCheckpoint[]> GetAllAsync(CancellationToken ct = default)
    {
        return await _dbContext.WalletIngestionCheckpoints.ToArrayAsync(ct);
    }

    public async Task SaveCheckpointsAsync(WalletIngestionCheckpoint checkpoint, CancellationToken ct = default)
    {
        await _dbContext.SingleMergeAsync(checkpoint, ct);
    }
}