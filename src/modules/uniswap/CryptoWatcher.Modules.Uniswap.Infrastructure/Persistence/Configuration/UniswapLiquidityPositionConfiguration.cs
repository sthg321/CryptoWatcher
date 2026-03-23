using CryptoWatcher.Modules.Uniswap.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Persistence.Configuration;

public class UniswapLiquidityPositionConfiguration : IEntityTypeConfiguration<UniswapLiquidityPosition>
{
    public void Configure(EntityTypeBuilder<UniswapLiquidityPosition> builder)
    {
        builder.HasKey(position => new { position.PositionId, position.NetworkName });

        builder.Property(network => network.NetworkName).HasMaxLength(32);
        
         builder.HasMany(position => position.Snapshots)
            .WithOne()
            .HasForeignKey(snapshot => new { snapshot.PoolPositionId, snapshot.NetworkName })
            .IsRequired();

        builder.HasMany(poolPosition => poolPosition.CashFlows)
            .WithOne()
            .HasForeignKey(position => new { position.PositionId, position.NetworkName })
            .IsRequired();
    }
}