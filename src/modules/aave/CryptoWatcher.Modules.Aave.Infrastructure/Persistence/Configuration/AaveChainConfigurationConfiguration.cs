using CryptoWatcher.Modules.Aave.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Configuration.Aave;

public class AaveChainConfigurationConfiguration : IEntityTypeConfiguration<AaveChainConfiguration>
{
    public void Configure(EntityTypeBuilder<AaveChainConfiguration> builder)
    {
        builder.HasKey(configuration => configuration.Name);

        builder.Property(configuration => configuration.Name).HasMaxLength(32);
        builder.Property(configuration => configuration.RpcAuthToken).HasMaxLength(64);

        builder.ComplexProperty(configuration => configuration.SmartContractAddresses);
    }
}