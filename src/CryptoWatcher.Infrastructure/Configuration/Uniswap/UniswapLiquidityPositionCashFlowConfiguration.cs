using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Configuration.Uniswap;

public class UniswapLiquidityPositionCashFlowConfiguration : IEntityTypeConfiguration<UniswapLiquidityPositionCashFlow>
{
    public void Configure(EntityTypeBuilder<UniswapLiquidityPositionCashFlow> builder)
    {
        builder.HasKey(flow => new { flow.PositionId, flow.NetworkName, flow.TransactionHash });

        builder.OwnsOne<TokenInfoWithFee>(flow => flow.Token0);
        
        builder.OwnsOne<TokenInfoWithFee>(flow => flow.Token1); 

        builder.Property(flow => flow.TransactionHash)
            .HasConversion(hash => hash.Value, s => TransactionHash.FromString(s))
            .HasMaxLength(66);
    }
}