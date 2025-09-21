using CryptoWatcher.AaveModule.Entities;
using CryptoWatcher.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Configuration.Aave;

public class AavePositionEventConfiguration : IEntityTypeConfiguration<AavePositionEvent>
{
    public void Configure(EntityTypeBuilder<AavePositionEvent> builder)
    {
        builder.HasKey(@event => new { @event.PositionId, @event.Date, EventType = @event.Event });

        builder.OwnsOne<TokenInfo>(position => position.Token);
    }
}