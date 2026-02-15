using CryptoWatcher.Modules.Hyperliquid.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Modules.Hyperliquid.Infrastructure.Persistence.Configuration;

public class HyperliquidVaultPositionConfiguration : IEntityTypeConfiguration<HyperliquidVaultPosition>
{
    public void Configure(EntityTypeBuilder<HyperliquidVaultPosition> builder)
    {
        builder.HasKey(position => new { position.VaultAddress, position.WalletAddress });

        builder.Navigation(position => position.Snapshots)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(position => position.CashFlows)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(position => position.Snapshots)
            .WithOne()
            .HasForeignKey(snapshot => new { snapshot.VaultAddress, snapshot.WalletAddress })
            .IsRequired();

        builder.HasMany(position => position.CashFlows)
            .WithOne()
            .HasForeignKey(snapshot => new { snapshot.VaultAddress, snapshot.WalletAddress })
            .IsRequired();

        builder.HasMany(position => position.Periods)
            .WithOne()
            .HasForeignKey(period => new { period.VaultAddress, period.WalletAddress })
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}