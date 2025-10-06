using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Configuration.Uniswap;

public class PoolPositionConfiguration : IEntityTypeConfiguration<PoolPosition>
{
    public void Configure(EntityTypeBuilder<PoolPosition> builder)
    {
        builder.HasKey(position => new { position.PositionId, position.NetworkName });

        builder.Property(network => network.NetworkName).HasMaxLength(32);
        builder.Property(network => network.WalletAddress).HasMaxLength(64);

        builder.HasOne(x => x.Wallet)
            .WithMany()
            .HasForeignKey(position => position.WalletAddress)
            .IsRequired();

        builder.HasMany(poolPosition => poolPosition.CashFlows)
            .WithOne()
            .HasForeignKey(position => new { position.PositionId, position.NetworkName })
            .IsRequired();

        builder.OwnsOne<TokenInfo>(position => position.Token0);
        builder.OwnsOne<TokenInfo>(position => position.Token1); 
    }
}