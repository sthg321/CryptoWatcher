using CryptoWatcher.Models;
using CryptoWatcher.Shared.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Models;

/// <summary>
/// Represents a report that summarizes the position of a Uniswap pool.
/// This includes information such as the total value of the pool in USD, total holding in USD,
/// total fees earned in USD, and a collection of individual pool position details.
/// </summary>
public class UniswapDailyReport : PlatformDailyReport
{
    public string NetworkName => ReportItems.FirstOrDefault()?.Network ?? string.Empty;

    /// <summary>
    /// Gets the total hold value in USD derived from the Uniswap pool positions.
    /// This value represents the aggregate monetary value of the tokens held in the pool positions.
    /// </summary>
    public required Money TotalHoldInUsd { get; init; }

    /// <summary>
    /// Gets the total commission value in USD accumulated from Uniswap pool positions.
    /// This value represents the aggregate fees earned through liquidity provisions within the pools.
    /// </summary>
    public Money TotalCommissionInUsd => ReportItems.Sum(item => item.DailyProfitInUsd);
    
    public Money TotalRewardsInUsd => ReportItems.Sum(item => item.RewardsInUsd);

    /// <summary>
    /// Gets the collection of report items that represent individual snapshots of a Uniswap pool position.
    /// Each report item includes detailed information such as the network, date, token pair,
    /// position values in USD, hold values in USD, and fee values in USD.
    /// </summary>
    public required IReadOnlyCollection<UniswapDailyReportItem> ReportItems { get; init; } = [];

    public override string GetNeworkName()
    {
        return NetworkName;
    }
}