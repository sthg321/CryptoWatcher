using CryptoWatcher.Modules.Hyperliquid.Entities;
using CryptoWatcher.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Configuration.Hyperliquid;

public class HyperliquidVaultEventConfiguration : IEntityTypeConfiguration<HyperliquidVaultEvent>
{
    public void Configure(EntityTypeBuilder<HyperliquidVaultEvent> builder)
    {
        builder.HasKey(@event => new { @event.VaultAddress, @event.WalletAddress, @event.Date });
        
        builder.Property(wallet => wallet.WalletAddress).ConfigureEvmAddress();
        builder.Property(wallet => wallet.VaultAddress).ConfigureEvmAddress();
    }
}