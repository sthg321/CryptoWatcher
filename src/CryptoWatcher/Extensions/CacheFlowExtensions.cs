using CryptoWatcher.Abstractions.CacheFlows;

namespace CryptoWatcher.Extensions;

public static class CacheFlowExtensions
{
    public static decimal CalculateNetCashFlowInUsd(this IEnumerable<IUsdCacheFlow> cacheFlows, DateOnly from,
        DateOnly to)
    {
        return cacheFlows.Where(e => FilterCashFlowEvents(e, from, to)).Sum(e => ComputeCashFlowEvent(e.Event, e.Usd));
    }
 
    private static bool FilterCashFlowEvents(ICacheFlow cacheFlow, DateOnly from, DateOnly to)
    {
        return cacheFlow.Date >= from.ToMinDateTime() && cacheFlow.Date <= to.ToMaxDateTime();
    }

    private static decimal ComputeCashFlowEvent(CacheFlowEvent @event, decimal amount)
    {
        if (@event == CacheFlowEvent.Deposit)
        {
            return amount;
        }

        return -amount;
    }
}