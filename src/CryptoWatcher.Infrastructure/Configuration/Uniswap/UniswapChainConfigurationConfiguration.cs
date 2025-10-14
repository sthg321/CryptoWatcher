using System.Numerics;
using CryptoWatcher.Infrastructure.Extensions;
using CryptoWatcher.Modules.Uniswap.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Configuration.Uniswap;

public class UniswapChainConfigurationConfiguration : IEntityTypeConfiguration<UniswapChainConfiguration>
{
    public void Configure(EntityTypeBuilder<UniswapChainConfiguration> builder)
    {
        builder.HasKey(network => new { network.Name, network.ProtocolVersion });

        builder.Property(network => network.Name).HasMaxLength(32);
        builder.Property(network => network.RpcUrl)
            .HasConversion(uri => uri.ToString(), uriString => new Uri(uriString))
            .HasMaxLength(128);
        
        builder.Property(network => network.RpcAuthToken).HasMaxLength(128);
        builder.Property(network => network.BlockscoutUrl)
            .HasConversion(uri => uri.ToString(), uriString => new Uri(uriString))
            .HasMaxLength(128);

        builder.Property(configuration => configuration.LastProcessedBlock)
            .HasConversion(integer => integer.ToString(), bigInterString => BigInteger.Parse(bigInterString));
        
        builder.OwnsOne(chain => chain.SmartContractAddresses, navigationBuilder =>
        {
            navigationBuilder.Property(addresses => addresses.NftManager).ConfigureEvmAddress();
            navigationBuilder.Property(addresses => addresses.PoolFactory).ConfigureEvmAddress();
            navigationBuilder.Property(addresses => addresses.MultiCall).ConfigureEvmAddress();
            navigationBuilder.Property(addresses => addresses.PositionManager).ConfigureEvmAddress();
        });

        builder.Navigation(configuration => configuration.LiquidityPoolPositions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        builder.HasMany(chainConfiguration => chainConfiguration.LiquidityPoolPositions)
            .WithOne()
            .HasForeignKey(position => new { position.NetworkName, position.ProtocolVersion })
            .IsRequired();
    }
}