using CryptoWatcher.Shared.ValueObjects;

namespace CryptoWatcher.AaveModule.Models;

public class AavePositionReportItem
{
    /// <summary>
    /// Gets the day for which the Aave position report is generated.
    /// </summary>
    /// <remarks>
    /// This property indicates the specific calendar day associated with the report,
    /// providing context for the recorded balance, profit, and performance metrics.
    /// </remarks>
    public required DateOnly Day { get; init; }

    /// <summary>
    /// Gets or sets the detailed information about the token associated with the Aave position.
    /// </summary>
    /// <remarks>
    /// This property encapsulates the token's details, including its symbol, amount, and valuation in USD,
    /// providing comprehensive context about the specific asset in the position.
    /// </remarks>
    public required TokenInfo Token { get; init; }

    /// <summary>
    /// Gets the usd profit earned within a single day for the Aave position report.
    /// </summary>
    /// <remarks>
    /// This property represents the monetary gain achieved over the specified day,
    /// reflecting the performance of the position for that time period.
    /// </remarks>
    public required Percent DailyProfitInUsd { get; init; }
    
    /// <summary>
    /// Gets the token profit earned within a single day for the Aave position report.
    /// </summary>
    /// <remarks>
    /// This property represents the monetary gain achieved over the specified day,
    /// reflecting the performance of the position for that time period.
    /// </remarks>
    public required decimal DailyTokenInProfit { get; init; }

    /// <summary>
    /// Gets the daily percentage profit for the Aave position.
    /// </summary>
    /// <remarks>
    /// This property represents the profit expressed as a percentage of the total balance
    /// for a specific day, offering insight into the daily performance of the position.
    /// </remarks>
    public required Percent DailyPercentProfitInUsd { get; init; }
}