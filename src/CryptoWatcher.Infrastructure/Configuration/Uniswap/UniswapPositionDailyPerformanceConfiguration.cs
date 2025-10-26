using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Configuration.Uniswap;

public class UniswapPositionDailyPerformanceConfiguration : IEntityTypeConfiguration<UniswapPositionDailyPerformance>
{
    public void Configure(EntityTypeBuilder<UniswapPositionDailyPerformance> builder)
    {
        builder.HasKey(performance => new { performance.Day, performance.NetworkName, performance.PoolPositionId });
        
        builder.Property(performance => performance.NetworkName).HasMaxLength(32);

        builder.ComplexProperty<TokenInfoWithFee>(snapshot => snapshot.Token0,
            propertyBuilder => propertyBuilder.Property(info => info.Symbol).HasMaxLength(16));
        
        builder.ComplexProperty<TokenInfoWithFee>(snapshot => snapshot.Token1,
            propertyBuilder => propertyBuilder.Property(info => info.Symbol).HasMaxLength(16));
    }
}