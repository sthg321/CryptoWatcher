using CryptoWatcher.Modules.Aave.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Configuration.Aave;

public class AaveAccountSnapshotConfiguration : IEntityTypeConfiguration<AaveAccountSnapshot>
{
    public void Configure(EntityTypeBuilder<AaveAccountSnapshot> builder)
    {
        builder.HasKey(snapshot => new { snapshot.WalletAddress, snapshot.NetworkName, snapshot.Day });
        builder.Property(snapshot => snapshot.NetworkName).HasMaxLength(32);
    }
}