using CryptoWatcher.Modules.Uniswap.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Persistence.Configuration;

public class UniswapLiquidityPositionCashFlowConfiguration : IEntityTypeConfiguration<UniswapLiquidityPositionCashFlow>
{
    public void Configure(EntityTypeBuilder<UniswapLiquidityPositionCashFlow> builder)
    {
        builder.HasKey(flow => new { flow.PositionId, flow.NetworkName, flow.TransactionHash, flow.Event }); 
    }
}