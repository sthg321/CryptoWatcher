using CryptoWatcher.Modules.Aave.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Configuration.Aave;

public class AavePositionEventConfiguration : IEntityTypeConfiguration<AavePositionCashFlow>
{
    public void Configure(EntityTypeBuilder<AavePositionCashFlow> builder)
    {
        builder.HasKey(@event => new { @event.PositionId, @event.Date, EventType = @event.Event });
    }
}