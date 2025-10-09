using System.Numerics;
using CryptoWatcher.Modules.Uniswap.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Configuration;

public class UniswapChainConfigurationConfiguration : IEntityTypeConfiguration<UniswapChainConfiguration>
{
    private const int AddressMaxLength = 42;

    public void Configure(EntityTypeBuilder<UniswapChainConfiguration> builder)
    {
        builder.HasKey(network => new { network.Name, network.ProtocolVersion });

        builder.Property(network => network.Name).HasMaxLength(32);
        builder.Property(network => network.RpcUrl).HasMaxLength(128);

        builder.Property(configuration => configuration.LastProcessedBlock)
            .HasConversion(integer => integer.ToString(), bigInterString => BigInteger.Parse(bigInterString));
        
        builder.OwnsOne(chain => chain.SmartContractAddresses, navigationBuilder =>
        {
            navigationBuilder.Property(addresses => addresses.NftManager).HasMaxLength(AddressMaxLength);
            navigationBuilder.Property(addresses => addresses.PoolFactory).HasMaxLength(AddressMaxLength);
            navigationBuilder.Property(addresses => addresses.MultiCall).HasMaxLength(AddressMaxLength);
        });

        builder.Navigation(configuration => configuration.LiquidityPoolPositions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        builder.HasMany(chainConfiguration => chainConfiguration.LiquidityPoolPositions)
            .WithOne()
            .HasForeignKey(position => new { position.NetworkName, position.ProtocolVersion })
            .IsRequired();
    }
}