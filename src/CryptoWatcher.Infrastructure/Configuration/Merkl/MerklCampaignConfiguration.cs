using CryptoWatcher.Infrastructure.Configuration.Conventions;
using CryptoWatcher.Modules.Merkl.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Configuration.Merkl;

public class MerklCampaignConfiguration : IEntityTypeConfiguration<MerklCampaign>
{
    public void Configure(EntityTypeBuilder<MerklCampaign> builder)
    {
        builder.Property(campaign => campaign.TokenSymbol).HasMaxLength(TokenInfoSymbolMaxLengthConvention.MaxLength);

        builder.Navigation(campaign => campaign.Snapshots).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}