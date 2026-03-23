using CryptoWatcher.Modules.Uniswap.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Persistence.Configuration;

public class UniswapPositionDailyPerformanceConfiguration : IEntityTypeConfiguration<UniswapPositionDailyPerformance>
{
    public void Configure(EntityTypeBuilder<UniswapPositionDailyPerformance> builder)
    {
        builder.HasKey(performance => new { performance.Day, performance.NetworkName, performance.PoolPositionId });
        
        builder.Property(performance => performance.NetworkName).HasMaxLength(32);
    }
}