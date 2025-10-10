using CryptoWatcher.HyperliquidModule.Entities;
using CryptoWatcher.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Configuration.Hyperliquid;

public class HyperliquidVaultPositionSnapshotConfiguration : IEntityTypeConfiguration<HyperliquidVaultPositionSnapshot>
{
    public void Configure(EntityTypeBuilder<HyperliquidVaultPositionSnapshot> builder)
    {
        builder.HasKey(snapshot => new { snapshot.VaultAddress, snapshot.WalletAddress, Date = snapshot.Day });

        builder.Property(wallet => wallet.VaultAddress).HasMaxLength(64);
        
        builder.Property(wallet => wallet.WalletAddress).ConfigureEvmAddress();
    }
}