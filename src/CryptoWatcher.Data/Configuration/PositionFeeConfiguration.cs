using CryptoWatcher.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Data.Configuration;

public class PositionFeeConfiguration : IEntityTypeConfiguration<PoolPositionFee>
{
    public void Configure(EntityTypeBuilder<PoolPositionFee> builder)
    {
        builder.Property(poolPositionHistory => poolPositionHistory.NetworkName).HasMaxLength(32);

        builder.HasKey(fee => new { fee.LiquidityPoolPositionId, fee.NetworkName, fee.Day });

        builder.HasOne(positionFee => positionFee.PoolPosition)
            .WithMany(position => position.PositionFees)
            .HasForeignKey(fee => new { fee.LiquidityPoolPositionId, fee.NetworkName,  fee.Day })
            .IsRequired();
        
        builder.OwnsOne<TokenInfo>(snapshot => snapshot.Token0Fee);
        builder.OwnsOne<TokenInfo>(snapshot => snapshot.Token1Fee);
    }
}