using CryptoWatcher.Modules.Morpho.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Configuration.Morpho;

public class MorphoMarketPositionConfiguration : IEntityTypeConfiguration<MorphoMarketPosition>
{
    public void Configure(EntityTypeBuilder<MorphoMarketPosition> builder)
    {
        builder.HasMany(position => position.Snapshots)
            .WithOne()
            .HasForeignKey(position => position.MorphoMarketPositionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}