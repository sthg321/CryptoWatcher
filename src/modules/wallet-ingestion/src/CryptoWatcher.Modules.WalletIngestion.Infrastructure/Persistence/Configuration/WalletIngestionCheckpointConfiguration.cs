using CryptoWatcher.Modules.WalletIngestion.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Modules.WalletIngestion.Infrastructure.Persistence.Configuration;

public class WalletIngestionCheckpointConfiguration : IEntityTypeConfiguration<WalletIngestionCheckpoint>
{
    public void Configure(EntityTypeBuilder<WalletIngestionCheckpoint> builder)
    {
        builder.HasKey(checkpoint => new { checkpoint.WalletAddress, checkpoint.ChainId });
    }
}