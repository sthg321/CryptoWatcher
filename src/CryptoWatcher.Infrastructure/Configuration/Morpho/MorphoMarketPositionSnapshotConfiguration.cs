using CryptoWatcher.Modules.Morpho.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Configuration.Morpho;

public class MorphoMarketPositionSnapshotConfiguration : IEntityTypeConfiguration<MorphoMarketPositionSnapshot>
{
    public void Configure(EntityTypeBuilder<MorphoMarketPositionSnapshot> builder)
    {
        builder.HasKey(snapshot => new { snapshot.Day, snapshot.MorphoMarketPositionId });
    }
}