using Ardalis.Specification;
using CryptoWatcher.Modules.Morpho.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Morpho.Specifications;

public class MorphoPositionStatusSpecification : Specification<MorphoMarketPosition>
{
    public MorphoPositionStatusSpecification(IReadOnlyCollection<EvmAddress> walletAddress, DateOnly day)
    {
        Query
            .Where(position => walletAddress.Contains(position.WalletAddress) && position.ClosedAt == null)
            .Include(position => position.Snapshots.Where(snapshot => snapshot.Day == day));
    }
}