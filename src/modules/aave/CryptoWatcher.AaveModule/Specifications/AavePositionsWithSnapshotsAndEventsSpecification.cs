using Ardalis.Specification;
using CryptoWatcher.AaveModule.Entities;
using CryptoWatcher.Extensions;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.AaveModule.Specifications;

internal class AavePositionsWithSnapshotsAndEventsSpecification : Specification<AavePosition>
{
    public AavePositionsWithSnapshotsAndEventsSpecification(IEnumerable<Wallet> wallets, DateOnly from, DateOnly to)
    {
        var walletAddresses = wallets.Select(wallet => wallet.Address).ToArray();
        Query
            .Include(position => position.Wallet)
            .Include(position => position.PositionEvents.Where(@event =>
                    @event.Date >= from.ToMinDateTime() && @event.Date <= to.ToMaxDateTime())
                .OrderBy(@event => @event.Date)
            )
            .Include(position =>
                position.PositionSnapshots.Where(snapshot => snapshot.Day >= from && snapshot.Day <= to))
            .Where(position => walletAddresses.Contains(position.WalletAddress));
    }
}