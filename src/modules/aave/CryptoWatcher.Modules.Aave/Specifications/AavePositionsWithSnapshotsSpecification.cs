using Ardalis.Specification;
using CryptoWatcher.Modules.Aave.Entities;

namespace CryptoWatcher.Modules.Aave.Specifications;

/// <summary>
/// Specification that defines criteria for retrieving Aave positions with their associated snapshots
/// for a specific wallet address within a given date range.
/// </summary>
/// <remarks>
/// This class is used to query and include Aave position snapshots filtered by their dates, along with
/// positions for a particular wallet address.
/// </remarks>
public sealed class AavePositionsWithSnapshotsSpecification : Specification<AavePosition>
{
    public AavePositionsWithSnapshotsSpecification(string walletAddress, DateOnly from, DateOnly to)
    {
        Query.Include(position =>
                position.PositionSnapshots.Where(snapshot => snapshot.Day >= from && snapshot.Day <= to))
            .Where(position => position.WalletAddress == walletAddress && position.ClosedAtDay == null);

    }
}