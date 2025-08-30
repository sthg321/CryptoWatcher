using CryptoWatcher.UniswapModule.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Configuration;

public class NetworkConfiguration : IEntityTypeConfiguration<UniswapNetwork>
{
    public void Configure(EntityTypeBuilder<UniswapNetwork> builder)
    {
        builder.HasKey(network => network.Name);

        builder.Property(network => network.Name).HasMaxLength(32);
        builder.Property(network => network.RpcUrl).HasMaxLength(128);
        builder.Property(network => network.MultiCallAddress).HasMaxLength(256);
        builder.Property(network => network.NftManagerAddress).HasMaxLength(266);
        builder.Property(network => network.PoolFactoryAddress).HasMaxLength(256);

        builder.HasMany(x => x.LiquidityPoolPositions)
            .WithOne(position => position.UniswapNetwork)
            .HasForeignKey(position => position.NetworkName)
            .IsRequired();
    }
}