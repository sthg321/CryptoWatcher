using CryptoWatcher.Shared.ValueObjects;

namespace CryptoWatcher.AaveModule.Models;

/// <summary>
/// Represents a report for a position in Aave, including balance, token details, and daily-profit metrics.
/// </summary>
public class AavePositionReport
{
    /// <summary>
    /// Gets the profit earned within a single day for the Aave position report.
    /// </summary>
    /// <remarks>
    /// This property represents the monetary gain achieved over the specified day,
    /// reflecting the performance of the position for that time period.
    /// </remarks>
    public Money TotalDailyProfitInUsd => ReportItems.Sum(item => item.DailyProfitInUsd);
 
    /// <summary>
    /// 
    /// </summary>
    public decimal TotalDailyProfitInToken => ReportItems.Sum(item => item.DailyPercentProfitInUsd);

    /// <summary>
    /// Gets the daily percentage profit for the Aave position.
    /// </summary>
    /// <remarks>
    /// This property represents the profit expressed as a percentage of the total balance
    /// for a specific day, offering insight into the daily performance of the position.
    /// </remarks>
    public Percent TotalDailyPercentProfitInToken => ReportItems.Average(item => item.DailyPercentProfitInUsd);

    public IReadOnlyCollection<AavePositionReportItem> ReportItems { get; set; } = [];
}