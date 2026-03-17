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
    
        builder.Property(network => network.RpcAuthToken).HasMaxLength(128);

        builder.Property(configuration => configuration.LastProcessedBlock)
            .HasConversion(integer => integer.ToString(), bigInterString => BigInteger.Parse(bigInterString));

        builder.ComplexProperty(chain => chain.SmartContractAddresses, propertyBuilder => propertyBuilder.Ignore(addresses => addresses.ProtocolVersion));

        builder.Navigation(configuration => configuration.LiquidityPoolPositions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        builder.HasMany(chainConfiguration => chainConfiguration.LiquidityPoolPositions)
            .WithOne()
            .HasForeignKey(position => new { position.NetworkName, position.ProtocolVersion })
            .IsRequired();

        builder.Ignore(configuration => configuration.SmartContractAddressesList);
    }
}