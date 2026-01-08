using CryptoWatcher.Modules.Merkl.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Configuration.Merkl;

public class MerklCampaignSnapshotConfiguration : IEntityTypeConfiguration<MerklCampaignSnapshot>
{
    public void Configure(EntityTypeBuilder<MerklCampaignSnapshot> builder)
    {
        builder.HasKey(snapshot => new { snapshot.Day, snapshot.MerklCampaignId });

        builder.Property(snapshot => snapshot.Day);
    }
}