using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Configuration.Uniswap;

public class UniswapLiquidityPositionSnapshotConfiguration : IEntityTypeConfiguration<UniswapLiquidityPositionSnapshot>
{
    public void Configure(EntityTypeBuilder<UniswapLiquidityPositionSnapshot> builder)
    {
        builder.Property(poolPositionHistory => poolPositionHistory.NetworkName).HasMaxLength(32);

        builder.HasKey(fee => new { fee.PoolPositionId, fee.NetworkName, fee.Day });

        builder.HasOne(positionFee => positionFee.UniswapLiquidityPosition)
            .WithMany(position => position.PoolPositionSnapshots)
            .HasForeignKey(fee => new { fee.PoolPositionId, fee.NetworkName })
            .IsRequired();
        
        builder.ComplexProperty<TokenInfoWithFee>(snapshot => snapshot.Token0,
            propertyBuilder => propertyBuilder.Property(info => info.Symbol).HasMaxLength(16));
        
        builder.ComplexProperty<TokenInfoWithFee>(snapshot => snapshot.Token1,
            propertyBuilder => propertyBuilder.Property(info => info.Symbol).HasMaxLength(16));
    }
}