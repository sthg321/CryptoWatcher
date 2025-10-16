using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Configuration.Aave;

public class AavePositionSnapshotConfiguration : IEntityTypeConfiguration<AavePositionSnapshot>
{
    public void Configure(EntityTypeBuilder<AavePositionSnapshot> builder)
    {
        builder.HasKey(snapshot => new { snapshot.PositionId, snapshot.Day });

        builder.ComplexProperty<TokenInfo>(snapshot => snapshot.Token).Property(info => info.Symbol).HasMaxLength(16);
    }
}