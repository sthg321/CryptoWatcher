using CryptoWatcher.Infrastructure.Extensions;
using CryptoWatcher.Modules.Uniswap.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Configuration.Uniswap;

public class UniswapSynchronizationStateConfiguration : IEntityTypeConfiguration<UniswapSynchronizationState>
{
    public void Configure(EntityTypeBuilder<UniswapSynchronizationState> builder)
    {
        builder.HasKey(state => new { state.ChainName, state.UniswapProtocolVersion, state.WalletAddress });
        
        builder.Property(network => network.ChainName).HasMaxLength(32);
        builder.Property(state => state.WalletAddress).ConfigureEvmAddress();
        builder.Property(state => state.LastTransactionHash).ConfigureTransactionHash();

        builder.HasOne(state => state.Wallet)
            .WithMany()
            .HasForeignKey(state => state.WalletAddress)
            .IsRequired();

        builder.HasOne(state => state.ChainConfiguration)
            .WithMany()
            .HasForeignKey(state => new { state.ChainName, state.UniswapProtocolVersion })
            .IsRequired();
    }
}