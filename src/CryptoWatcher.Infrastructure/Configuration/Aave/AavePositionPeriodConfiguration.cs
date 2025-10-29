using CryptoWatcher.Modules.Aave.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Configuration.Aave;

public class AavePositionPeriodConfiguration :IEntityTypeConfiguration<AavePositionPeriod>
{
    public void Configure(EntityTypeBuilder<AavePositionPeriod> builder)
    {
        builder.HasIndex(period => new { period.PositionId, period.ClosedAtDay })
            .HasFilter($""" "{nameof(AavePositionPeriod.ClosedAtDay)}" IS NULL""")
            .IsUnique();
    }
}