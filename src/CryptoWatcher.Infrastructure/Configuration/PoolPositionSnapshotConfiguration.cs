using CryptoWatcher.UniswapModule.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Configuration;

public class PoolPositionSnapshotConfiguration : IEntityTypeConfiguration<PoolPositionSnapshot>
{
    public void Configure(EntityTypeBuilder<PoolPositionSnapshot> builder)
    {
        builder.Property(poolPositionHistory => poolPositionHistory.NetworkName).HasMaxLength(32);

        builder.HasKey(fee => new { fee.PoolPositionId, fee.NetworkName, fee.Day });

        builder.HasOne(positionFee => positionFee.PoolPosition)
            .WithMany(position => position.PoolPositionSnapshots)
            .HasForeignKey(fee => new { fee.PoolPositionId, fee.NetworkName })
            .IsRequired();
        
        builder.OwnsOne<TokenInfoWithFee>(snapshot => snapshot.Token0);
        builder.OwnsOne<TokenInfoWithFee>(snapshot => snapshot.Token1);
    }
}