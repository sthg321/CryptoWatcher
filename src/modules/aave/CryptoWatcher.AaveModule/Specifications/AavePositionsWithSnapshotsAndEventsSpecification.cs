using Ardalis.Specification;
using CryptoWatcher.AaveModule.Entities;
using CryptoWatcher.Extensions;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.AaveModule.Specifications;

internal class AavePositionsWithSnapshotsAndEventsSpecification : Specification<AavePosition>
{
    public AavePositionsWithSnapshotsAndEventsSpecification(Wallet wallet, DateOnly from, DateOnly to)
    {
        Query
            .Include(position => position.PositionEvents.Where(@event =>
                @event.Date >= from.ToMinDateTime() && @event.Date <= to.ToMaxDateTime()))
            .Include(position =>
                position.PositionSnapshots.Where(snapshot => snapshot.Day >= from && snapshot.Day <= to))
            .Where(position => position.WalletAddress == wallet.Address);
    }
}