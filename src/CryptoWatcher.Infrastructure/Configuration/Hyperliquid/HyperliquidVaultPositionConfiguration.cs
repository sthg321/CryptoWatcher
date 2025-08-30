using CryptoWatcher.HyperliquidModule.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Configuration.Hyperliquid;

public class HyperliquidVaultPositionConfiguration : IEntityTypeConfiguration<HyperliquidVaultPosition>
{
    public void Configure(EntityTypeBuilder<HyperliquidVaultPosition> builder)
    {
        builder.HasKey(position => new { position.VaultAddress, position.WalletAddress });

        builder.Property(wallet => wallet.WalletAddress).HasMaxLength(64);
        builder.Property(wallet => wallet.VaultAddress).HasMaxLength(64);

        builder.HasMany(position => position.PositionSnapshots)
            .WithOne(snapshot => snapshot.Vault)
            .HasForeignKey(snapshot => new { snapshot.VaultAddress, snapshot.WalletAddress })
            .IsRequired();

        builder.HasMany(position => position.VaultEvents)
            .WithOne()
            .HasForeignKey(snapshot => new { snapshot.VaultAddress, snapshot.WalletAddress })
            .IsRequired();
    }
}