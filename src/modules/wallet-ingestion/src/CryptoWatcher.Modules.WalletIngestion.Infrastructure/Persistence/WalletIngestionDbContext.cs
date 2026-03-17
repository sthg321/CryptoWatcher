using CryptoWatcher.Modules.Infrastructure.Shared.Persistence;
using CryptoWatcher.Modules.WalletIngestion.Entities;
using Microsoft.EntityFrameworkCore;

namespace CryptoWatcher.Modules.WalletIngestion.Infrastructure.Persistence;

public class WalletIngestionDbContext : BaseDbContext
{
    public WalletIngestionDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<WalletIngestionCheckpoint> WalletIngestionCheckpoints => Set<WalletIngestionCheckpoint>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("wallet_ingestion");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WalletIngestionDbContext).Assembly);
    }
}