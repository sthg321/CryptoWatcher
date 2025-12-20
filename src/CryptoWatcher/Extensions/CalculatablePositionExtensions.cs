using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Abstractions.PositionSnapshots;
using CryptoWatcher.Models;

namespace CryptoWatcher.Extensions;

public static class CalculatablePositionExtensions
{
    public static ProfitMetric CalculateProfitInToken(
        this IDeFiPosition<ITokenPositionSnapshot, ITokenCashFlow> position, DateOnly from, DateOnly to)
    {
        return CalculateProfit(
            position,
            from,
            to,
            snapshot => snapshot.Token0.Amount,
            (cashFlow, _) => (cashFlow as ITokenCashFlow)!.Token0.Amount
        );
    }

    public static ProfitMetric CalculateProfitInUsd(
        this IDeFiPosition<ITokenPositionSnapshot, ITokenCashFlow> position, DateOnly from, DateOnly to)
    {
        return CalculateProfit(
            position,
            from,
            to,
            snapshot => snapshot.Token0.AmountInUsd,
            (cashFlow, _) => (cashFlow as ITokenCashFlow)!.Token0.AmountInUsd
        );
    }

    public static ProfitMetric CalculateProfitInUsd(
        this IDeFiPosition<ITokenPairPositionSnapshot, ITokenPairCashFlow> position, DateOnly from, DateOnly to)
    {
        return CalculateProfit(
            position,
            from,
            to,
            snapshot => snapshot.Token0.Fee * snapshot.Token0.PriceInUsd +
                        snapshot.Token1.Fee * snapshot.Token1.PriceInUsd,
            (cashFlow, snapshot) =>
            {
                var cashFlowPair = (cashFlow as ITokenPairCashFlow);

                return cashFlowPair!.Token0.Amount * snapshot.Token0.PriceInUsd +
                       cashFlowPair.Token1.Amount * snapshot.Token1.PriceInUsd;
            });
    }

    private static ProfitMetric CalculateProfit<TSnapshot, TCashFlow>(
        IDeFiPosition<TSnapshot, TCashFlow> position,
        DateOnly from,
        DateOnly to,
        Func<TSnapshot, decimal> getValue,
        Func<ICashFlow, TSnapshot, decimal> getCashFlowAmount)
        where TSnapshot : IPositionSnapshot
        where TCashFlow : ICashFlow
    {
        var snapshots = position.PositionSnapshots;
        var cashFlows = position.CashFlows;

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
                return cacheFlow.Event == CashFlowEvent.Deposit
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