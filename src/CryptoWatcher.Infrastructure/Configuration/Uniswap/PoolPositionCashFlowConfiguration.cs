using CryptoWatcher.UniswapModule.Entities;
using CryptoWatcher.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Configuration.Uniswap;

public class PoolPositionCashFlowConfiguration : IEntityTypeConfiguration<PoolPositionCashFlow>
{
    public void Configure(EntityTypeBuilder<PoolPositionCashFlow> builder)
    {
        builder.OwnsOne<TokenInfoWithFee>(flow => flow.Token0);
        
        builder.OwnsOne<TokenInfoWithFee>(flow => flow.Token1);
    }
}