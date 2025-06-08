using CryptoWatcher.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Data.Configuration;

public class LiquidityPoolPositionSnapshotConfiguration : IEntityTypeConfiguration<PositionFee>
{
    public void Configure(EntityTypeBuilder<PositionFee> builder)
    {
        builder.Property(poolPositionHistory => poolPositionHistory.NetworkName).HasMaxLength(32);

        builder.HasKey(history => new { history.NetworkName, history.LiquidityPoolPositionId, history.Day });

        builder.HasOne(liquidityPoolPositionHistory => liquidityPoolPositionHistory.PoolPosition)
            .WithMany(position => position.PositionFees)
            .HasForeignKey(history => new { history.LiquidityPoolPositionId, history.NetworkName })
            .IsRequired();
        
        builder.OwnsOne<TokenInfo>(snapshot => snapshot.Token0Fee);
        builder.OwnsOne<TokenInfo>(snapshot => snapshot.Token1Fee);
    }
}