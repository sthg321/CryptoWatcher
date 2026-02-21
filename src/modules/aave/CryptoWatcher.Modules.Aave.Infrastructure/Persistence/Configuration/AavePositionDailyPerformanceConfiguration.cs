using CryptoWatcher.Modules.Aave.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Configuration.Aave;

public class AavePositionDailyPerformanceConfiguration : IEntityTypeConfiguration<AavePositionDailyPerformance>
{
    public void Configure(EntityTypeBuilder<AavePositionDailyPerformance> builder)
    {
        builder.HasKey(performance =>
            new { performance.Day, performance.PositionType, performance.SnapshotPositionId });

        builder.Property(performance => performance.NetworkName).HasMaxLength(32);
    }
}