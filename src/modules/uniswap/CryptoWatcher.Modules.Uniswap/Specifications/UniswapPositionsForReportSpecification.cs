using Ardalis.Specification;
using CryptoWatcher.Extensions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Uniswap.Specifications;

/// <summary>
/// A specification for querying Uniswap pool positions within a specified date range for reporting purposes.
/// </summary>
/// <remarks>
/// This specification filters <see cref="UniswapLiquidityPosition"/> entities to include only those snapshots
/// whose date falls within the provided range. The included snapshots are ordered by their date.
/// </remarks>
public sealed class UniswapPositionsForReportSpecification : Specification<UniswapLiquidityPosition>
{
    public UniswapPositionsForReportSpecification(IReadOnlyCollection<Wallet> wallet, DateOnly from, DateOnly to)
    {
        var walletAddresses = wallet.Select(x => x.Address.Value).ToArray();
        Query
            .Include(position => position.Wallet)
            .Include(poolPosition => poolPosition.PositionSnapshots
                .Where(snapshot => snapshot.Day >= from && snapshot.Day <= to)
                .OrderBy(snapshot => snapshot.Day)
            )
            .Include(poolPosition => poolPosition.CashFlows)
            .Where(position => walletAddresses.Contains(position.WalletAddress));
    }
}