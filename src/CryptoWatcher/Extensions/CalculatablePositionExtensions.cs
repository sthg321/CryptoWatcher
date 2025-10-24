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
            position,
            from,
            to,
            snapshot => snapshot.GetUsdBalance(),
            (cashFlow, _) => (cashFlow as IUsdCacheFlow)!.Usd
        );
    }

    public static ProfitMetric CalculateProfitInToken(
        this ICalculatablePosition<ITokenPositionSnapshot> position, DateOnly from, DateOnly to)
    {
        return CalculateProfit(
            position,
            from,
            to,
            snapshot => snapshot.GetTokenInfo().Amount,
            (cashFlow, _) => (cashFlow as ITokenCacheFlow)!.Token.Amount
        );
    }

    public static ProfitMetric CalculateProfitInUsd(
        this ICalculatablePosition<ITokenPositionSnapshot> position, DateOnly from, DateOnly to)
    {
        return CalculateProfit(
            position,
            from,
            to,
            snapshot => snapshot.GetTokenInfo().AmountInUsd,
            (cashFlow, _) => (cashFlow as ITokenCacheFlow)!.Token.AmountInUsd
        );
    }

    public static ProfitMetric CalculateProfitInUsd(
        this ICalculatablePosition<ITokenPairPositionSnapshot> position, DateOnly from, DateOnly to)
    {
        return CalculateProfit(
            position,
            from,
            to,
            snapshot => snapshot.Token0.FeeAmount * snapshot.Token0.PriceInUsd +
                        snapshot.Token1.FeeAmount * snapshot.Token1.PriceInUsd,
            (cashFlow, snapshot) =>
            {
                var cashFlowPair = (cashFlow as ITokenPairCashFlow);

                return cashFlowPair!.Token0.Amount * snapshot.Token0.PriceInUsd +
                       cashFlowPair.Token1.Amount * snapshot.Token1.PriceInUsd;
            });
    }
    
    private static ProfitMetric CalculateProfit<TSnapshot>(
        ICalculatablePosition<TSnapshot> position,
        DateOnly from,
        DateOnly to,
        Func<TSnapshot, decimal> getValue,
        Func<ICacheFlow, TSnapshot, decimal> getCashFlowAmount)
        where TSnapshot : IPositionSnapshot
    {
        var snapshots = position.GetPositionSnapshots();
        var cashFlows = position.GetCashFlows();

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
            .Where(cashFlow => cashFlow.Date.ToDateOnly() > startSnapshot.Day && cashFlow.Date.ToDateOnly() <= to)
            .Sum(cacheFlow =>
            {
                var cashFlowDay = cacheFlow.Date.ToDateOnly();
                var snapshot = startSnapshot.Day == cashFlowDay ? startSnapshot : endSnapshot;
                return cacheFlow.Event == CacheFlowEvent.Deposit
                    ? getCashFlowAmount(cacheFlow, snapshot)
                    : -getCashFlowAmount(cacheFlow, snapshot);
            });

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