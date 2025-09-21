using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Abstractions.PositionSnapshots;
using CryptoWatcher.Extensions;

namespace CryptoWatcher.Abstractions;

public class ProfitCalculator
{
    public decimal CalculateAbsoluteProfit(
        IReadOnlyCollection<IUsdPositionSnapshot> snapshots,
        IReadOnlyCollection<IUsdCacheFlow> cashFlowEvents,
        DateOnly startDate,
        DateOnly endDate)
    {
        var startSnapshot = snapshots.GetNearestSnapshot(startDate, false);
        var endSnapshot = snapshots.GetNearestSnapshot(endDate, true);

        if (startSnapshot == null || endSnapshot == null)
            return 0;

        var netCashFlow = cashFlowEvents.CalculateNetCashFlowInUsd(startSnapshot.Day, endSnapshot.Day);

        if (startSnapshot.Day == endSnapshot.Day)
        {
            return endSnapshot.GetUsdBalance() - netCashFlow;
        }

        return endSnapshot.GetUsdBalance() - startSnapshot.GetUsdBalance() - netCashFlow;
    }
}