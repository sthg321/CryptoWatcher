using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Abstractions.PositionSnapshots;
using CryptoWatcher.Models;

namespace CryptoWatcher.Extensions;

public static class CalculatablePositionExtensions
{
    public static ProfitMetric CalculateProfitInUsd(
        this ICalculatablePosition<IUsdPositionSnapshot> position, DateOnly from, DateOnly to)
    {
        return CalculateProfit(
            position.GetPositionSnapshots(),
            position.GetCashFlows(),
            from,
            to,
            snapshot => snapshot.GetUsdBalance(),
            cashFlow => (cashFlow as IUsdCacheFlow)!.Usd
        );
    }

    public static ProfitMetric CalculateProfitInToken(
        this ICalculatablePosition<ITokenPositionSnapshot> position, DateOnly from, DateOnly to)
    {
        return CalculateProfit(
            position.GetPositionSnapshots(),
            position.GetCashFlows(),
            from,
            to,
            snapshot => snapshot.GetTokenInfo().Amount,
            cashFlow => (cashFlow as ITokenCacheFlow)!.Token.Amount
        );
    }

    public static ProfitMetric CalculateProfitInUsd(
        this ICalculatablePosition<ITokenPositionSnapshot> position, DateOnly from, DateOnly to)
    {
        return CalculateProfit(
            position.GetPositionSnapshots(),
            position.GetCashFlows(),
            from,
            to,
            snapshot => snapshot.GetTokenInfo().AmountInUsd,
            cashFlow => (cashFlow as ITokenCacheFlow)!.Token.AmountInUsd
        );
    }

    private static ProfitMetric CalculateProfit<TSnapshot>(
        IReadOnlyCollection<TSnapshot> snapshots,
        IReadOnlyCollection<ICacheFlow> cashFlows,
        DateOnly from,
        DateOnly to,
        Func<TSnapshot, decimal> getValue,
        Func<ICacheFlow, decimal> getCashFlowAmount)
        where TSnapshot : IPositionSnapshot
    {
        var filteredSnapshots = snapshots
            .Where(snapshot => snapshot.Day >= from && snapshot.Day <= to)
            .ToArray();

        if (filteredSnapshots.Length == 0)
        {
            return ProfitMetric.Empty();
        }

        var startSnapshot = filteredSnapshots.GetNearestSnapshot(from, false);
        var endSnapshot = filteredSnapshots.GetNearestSnapshot(to, true);

        if (startSnapshot == null || endSnapshot == null || startSnapshot.Day == endSnapshot.Day)
        {
            return ProfitMetric.Empty();
        }

        var filteredCashFlows = cashFlows
            .Where(cashFlow => cashFlow.Date.ToDateOnly() > from && cashFlow.Date.ToDateOnly() <= to)
            .Sum(cacheFlow => cacheFlow.Event == CacheFlowEvent.Deposit
                ? getCashFlowAmount(cacheFlow)
                : -getCashFlowAmount(cacheFlow));

        var startValue = getValue(startSnapshot);
        var endValue = getValue(endSnapshot);
        var profitValue = endValue - startValue - filteredCashFlows;

        if (profitValue == 0)
        {
            return ProfitMetric.Empty();
        }

        var denominator = Math.Abs(startValue);
        var profitPercent = denominator < 1e-9m ? 0 : profitValue / denominator;

        return new ProfitMetric { Amount = profitValue, Percent = profitPercent };
    }
}