using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.CacheFlows;

namespace CryptoWatcher.Extensions;

public static class CacheFlowExtensions
{
    public static decimal CalculateNetCashFlowInUsd(this IEnumerable<IUsdCacheFlow> cacheFlows, DateOnly from,
        DateOnly to)
    {
        return cacheFlows.Where(e => FilterCashFlowEvents(e, from, to)).Sum(e => ComputeCashFlowEvent(e.Event, e.Usd));
    }

    public static decimal CalculateNetCashFlowInUsd(this IEnumerable<ITokenCacheFlow> cacheFlows, DateOnly from,
        DateOnly to)
    {
        return cacheFlows.Where(e => FilterCashFlowEvents(e, from, to))
            .Sum(e => ComputeCashFlowEvent(e.Event, e.Token.AmountInUsd));
    }

    public static decimal CalculateNetCashFlowInToken(this IEnumerable<ITokenCacheFlow> cacheFlows, DateOnly from,
        DateOnly to)
    {
        return cacheFlows.Where(e => FilterCashFlowEvents(e, from, to))
            .Sum(e => ComputeCashFlowEvent(e.Event, e.Token.Amount));
    }

    private static bool FilterCashFlowEvents(ICacheFlow cacheFlow, DateOnly from, DateOnly to)
    {
        return cacheFlow.Date >= from.ToMinDateTime() && cacheFlow.Date <= to.ToMaxDateTime();
    }

    private static decimal ComputeCashFlowEvent(CacheFlowEvent @event, decimal amount)
    {
        return @event switch
        {
            CacheFlowEvent.Deposit => amount,
            CacheFlowEvent.Withdraw => -amount,
            _ => throw new ArgumentOutOfRangeException(nameof(@event))
        };
    }
}