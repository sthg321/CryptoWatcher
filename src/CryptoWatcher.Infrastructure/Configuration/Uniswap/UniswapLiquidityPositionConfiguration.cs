using CryptoWatcher.Infrastructure.Configuration.Converters;
using CryptoWatcher.Infrastructure.Extensions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Configuration.Uniswap;

public class UniswapLiquidityPositionConfiguration : IEntityTypeConfiguration<UniswapLiquidityPosition>
{
    public void Configure(EntityTypeBuilder<UniswapLiquidityPosition> builder)
    {
        builder.HasKey(position => new { position.PositionId, position.NetworkName });

        builder.Property(network => network.NetworkName).HasMaxLength(32);
        builder.Property(network => network.WalletAddress).HasMaxLength(64);

        builder.Property(wallet => wallet.WalletAddress).ConfigureEvmAddress();
        
        builder.HasOne(position => position.Wallet)
            .WithMany()
            .HasForeignKey(position => position.WalletAddress)
            .IsRequired();

        builder.HasMany(poolPosition => poolPosition.CashFlows)
            .WithOne()
            .HasForeignKey(position => new { position.PositionId, position.NetworkName })
            .IsRequired();

        builder.ComplexProperty<TokenInfo>(position => position.Token0);
        builder.ComplexProperty<TokenInfo>(position => position.Token1); 
    }
}