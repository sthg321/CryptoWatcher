using Ardalis.Specification;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.ValueObjects;

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
    public AavePositionsWithSnapshotsSpecification(IReadOnlyCollection<EvmAddress> wallets, DateOnly day)
    {
        Query.Include(position => position.PositionPeriods)
            .Include(position =>
                position.Snapshots.Where(snapshot => snapshot.Day >= day && snapshot.Day <= day))
            .Where(position => wallets.Contains(position.WalletAddress) && position.IsActive());
    }
    
    public AavePositionsWithSnapshotsSpecification(AaveChainConfiguration chain, Wallet wallet, DateOnly from,
        DateOnly to)
    {
        Query
            .Include(position => position.PositionPeriods)
            .Include(position =>
                position.Snapshots.Where(snapshot => snapshot.Day >= from && snapshot.Day <= to))
            .Where(position => position.WalletAddress == wallet.Address && position.IsActive() &&
                               position.Network == chain.Name);
    }
}