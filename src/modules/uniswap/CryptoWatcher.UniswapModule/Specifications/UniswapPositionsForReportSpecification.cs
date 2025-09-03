using Ardalis.Specification;
using CryptoWatcher.UniswapModule.Entities;

namespace CryptoWatcher.UniswapModule.Specifications;

/// <summary>
/// A specification for querying Uniswap pool positions within a specified date range for reporting purposes.
/// </summary>
/// <remarks>
/// This specification filters <see cref="PoolPosition"/> entities to include only those snapshots
/// whose date falls within the provided range. The included snapshots are ordered by their date.
/// </remarks>
internal sealed class UniswapPositionsForReportSpecification : Specification<PoolPosition>
{
    public UniswapPositionsForReportSpecification(DateOnly from, DateOnly to)
    {
        Query
            .Include(poolPosition => poolPosition.PoolPositionSnapshots
                .Where(snapshot => snapshot.Day >= from && snapshot.Day <= to)
                .OrderBy(snapshot => snapshot.Day)
            );
    }
}