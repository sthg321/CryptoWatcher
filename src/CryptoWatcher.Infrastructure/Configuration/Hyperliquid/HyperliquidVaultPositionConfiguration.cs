using CryptoWatcher.Modules.Hyperliquid.Entities;
using CryptoWatcher.Infrastructure.Configuration.Converters;
using CryptoWatcher.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Configuration.Hyperliquid;

public class HyperliquidVaultPositionConfiguration : IEntityTypeConfiguration<HyperliquidVaultPosition>
{
    public void Configure(EntityTypeBuilder<HyperliquidVaultPosition> builder)
    {
        builder.HasKey(position => new { position.VaultAddress, position.WalletAddress });

        builder.Property(wallet => wallet.WalletAddress).ConfigureEvmAddress();

        builder.Property(wallet => wallet.VaultAddress).ConfigureEvmAddress();

        builder.HasMany(position => position.PositionSnapshots)
            .WithOne()
            .HasForeignKey(snapshot => new { snapshot.VaultAddress, snapshot.WalletAddress })
            .IsRequired();

        builder.HasMany(position => position.VaultEvents)
            .WithOne()
            .HasForeignKey(snapshot => new { snapshot.VaultAddress, snapshot.WalletAddress })
            .IsRequired();
    }
}