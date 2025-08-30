using Ardalis.Specification;
using CryptoWatcher.HyperliquidModule.Entities;

namespace CryptoWatcher.HyperliquidModule.Specifications;

public sealed class HyperliquidPositionsForReportSpecification : Specification<HyperliquidVaultPosition>
{
    public HyperliquidPositionsForReportSpecification(DateOnly from, DateOnly to)
    {
        Query
            .Include(position => position.VaultEvents)
            .Include(position => position.PositionSnapshots
                .OrderBy(snapshot => snapshot.Day)
                .Where(snapshot => snapshot.Day >= from && snapshot.Day <= to));
    }
}