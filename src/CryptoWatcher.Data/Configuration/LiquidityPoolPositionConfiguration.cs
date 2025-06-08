using CryptoWatcher.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Data.Configuration;

public class LiquidityPoolPositionConfiguration : IEntityTypeConfiguration<PoolPosition>
{
    public void Configure(EntityTypeBuilder<PoolPosition> builder)
    {
        builder.HasKey(position => new { position.PositionId, position.NetworkName });
        
        builder.Property(network => network.NetworkName).HasMaxLength(32);
        builder.Property(network => network.WalletAddress).HasMaxLength(64);
        
        builder.OwnsOne<TokenInfo>(position => position.Token0);
        builder.OwnsOne<TokenInfo>(position => position.Token1);
    }
}