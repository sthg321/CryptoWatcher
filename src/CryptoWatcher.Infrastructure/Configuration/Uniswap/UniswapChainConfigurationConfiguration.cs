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

        builder.ComplexProperty(chain => chain.SmartContractAddresses, propertyBuilder => propertyBuilder.Ignore(addresses => addresses.ProtocolVersion));

        builder.Ignore(configuration => configuration.SmartContractAddressesList);
    }
}