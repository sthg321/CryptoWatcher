using CryptoWatcher.Abstractions.CacheFlows;

namespace CryptoWatcher.Extensions;

public static class CacheFlowExtensions
{
    public static decimal CalculateNetTokenCashFlowInUsd<TCashFlow>(this IEnumerable<TCashFlow> cacheFlows,
        DateOnly from,
        DateOnly to) where TCashFlow : ITokenCashFlow
    {
        return cacheFlows.Where(e => FilterCashFlowEvents(e, from, to))
            .Sum(e => ComputeCashFlowEvent(e.Event, e.Token.AmountInUsd));
    }
    
    public static decimal CalculateNetTokenCashFlowInToken<TCashFlow>(this IEnumerable<TCashFlow> cacheFlows,
        DateOnly from,
        DateOnly to) where TCashFlow : ITokenCashFlow
    {
        return cacheFlows.Where(e => FilterCashFlowEvents(e, from, to))
            .Sum(e => ComputeCashFlowEvent(e.Event, e.Token.Amount));
    }
    
    public static decimal CalculateNetTokenPairCashFlowInUsd<TCashFlow>(this IEnumerable<TCashFlow> cacheFlows,
        DateOnly from,
        DateOnly to) where TCashFlow : ITokenPairCashFlow
    {
        return cacheFlows.Where(e => FilterCashFlowEvents(e, from, to))
            .Sum(e => ComputeCashFlowEvent(e.Event, e.Token0.AmountInUsd + e.Token1.AmountInUsd));
    }

    private static bool FilterCashFlowEvents(ICashFlow cashFlow, DateOnly from, DateOnly to)
    {
        return cashFlow.Date >= from.ToMinDateTime() && cashFlow.Date <= to.ToMaxDateTime();
    }

    private static decimal ComputeCashFlowEvent(CashFlowEvent @event, decimal amount)
    {
        if (@event == CashFlowEvent.Deposit)
        {
            return amount;
        }

        return -amount;
    }
}