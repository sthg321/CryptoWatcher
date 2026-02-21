using CryptoWatcher.Modules.Aave.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Configuration.Aave;

public class AavePositionConfiguration : IEntityTypeConfiguration<AavePosition>
{
    public void Configure(EntityTypeBuilder<AavePosition> builder)
    {
        builder.Property(aavePosition => aavePosition.Network).HasMaxLength(32);
      
        builder.Navigation(position => position.Snapshots).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(position => position.CashFlows).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(position => position.PositionPeriods).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(aavePosition => aavePosition.PositionPeriods)
            .WithOne()
            .HasForeignKey(period => period.PositionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(aavePosition => aavePosition.Snapshots)
            .WithOne()
            .HasForeignKey(snapshot => snapshot.PositionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(aavePosition => aavePosition.CashFlows)
            .WithOne()
            .HasForeignKey(snapshot => snapshot.PositionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}