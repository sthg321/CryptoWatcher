using CryptoWatcher.Modules.Uniswap.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Persistence.Configuration;

public class UniswapLiquidityPositionSnapshotConfiguration : IEntityTypeConfiguration<UniswapLiquidityPositionSnapshot>
{
    public void Configure(EntityTypeBuilder<UniswapLiquidityPositionSnapshot> builder)
    {
        builder.Property(poolPositionHistory => poolPositionHistory.NetworkName).HasMaxLength(32);

        builder.HasKey(fee => new { fee.PoolPositionId, fee.NetworkName, fee.Day });
    }
}