using CryptoWatcher.UniswapModule.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Configuration;

public class PoolPositionConfiguration : IEntityTypeConfiguration<PoolPosition>
{
    public void Configure(EntityTypeBuilder<PoolPosition> builder)
    {
        builder.HasKey(position => new { PoolPositionId = position.PositionId, position.NetworkName });
        
        builder.Property(network => network.NetworkName).HasMaxLength(32);
        builder.Property(network => network.WalletAddress).HasMaxLength(64);
        
        builder.HasOne(x=>x.Wallet)
            .WithMany()
            .HasForeignKey(position => position.WalletAddress)
            .IsRequired();
        
        builder.OwnsOne<TokenInfo>(position => position.Token0);
        builder.OwnsOne<TokenInfo>(position => position.Token1);
    }
}