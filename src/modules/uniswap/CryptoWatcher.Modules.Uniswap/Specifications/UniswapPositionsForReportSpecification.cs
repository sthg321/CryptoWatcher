using Ardalis.Specification;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.UniswapModule.Entities;

namespace CryptoWatcher.UniswapModule.Specifications;

/// <summary>
/// A specification for querying Uniswap pool positions within a specified date range for reporting purposes.
/// </summary>
/// <remarks>
/// This specification filters <see cref="PoolPosition"/> entities to include only those snapshots
/// whose date falls within the provided range. The included snapshots are ordered by their date.
/// </remarks>
public sealed class UniswapPositionsForReportSpecification : Specification<PoolPosition>
{
    public UniswapPositionsForReportSpecification(IReadOnlyCollection<Wallet> wallet, DateOnly from, DateOnly to)
    {
        var walletAddresses = wallet.Select(x => x.Address).ToArray();
        Query
            .Include(position => position.Wallet)
            .Include(poolPosition => poolPosition.PoolPositionSnapshots
                .Where(snapshot => snapshot.Day >= from && snapshot.Day <= to)
                .OrderBy(snapshot => snapshot.Day)
            )
            .Where(position => walletAddresses.Contains(position.WalletAddress));
    }
}