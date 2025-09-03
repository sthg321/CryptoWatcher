namespace CryptoWatcher.HyperliquidModule.Models;

/// <summary>
/// Represents a report item for a specific vault in the Hyperliquid module.
/// </summary>
/// <remarks>
/// This record serves as a model to encapsulate the daily balance and profits associated with a vault.
/// </remarks>
public record HyperliquidVaultReportItem
{
    /// <summary>
    /// Gets the unique address of the vault associated with the report item.
    /// </summary>
    /// <remarks>
    /// This property identifies the address of the vault used within the Hyperliquid module
    /// and is critical for mapping vault-specific data, such as balances and profits, across reports.
    /// </remarks>
    public required string VaultAddress { get; init; } = null!;

    /// <summary>
    /// Gets the day associated with this report item.
    /// </summary>
    /// <remarks>
    /// This property indicates the specific date for which the balance, daily profit, and other metrics
    /// are calculated for the corresponding vault in the Hyperliquid module.
    /// </remarks>
    public required DateOnly Day { get; init; }

    /// <summary>
    /// Gets the balance associated with the vault on a specific day.
    /// </summary>
    /// <remarks>
    /// This property represents the total amount held in the vault for the given day
    /// as recorded in the Hyperliquid module.
    /// It is used to track vault performance
    /// and serves as a key metric for daily reports.
    /// </remarks>
    public required decimal Balance { get; init; }

    /// <summary>
    /// Gets the absolute daily profit for the specified vault.
    /// </summary>
    /// <remarks>
    /// This property represents the profit earned by the vault for a single day
    /// and is calculated as the difference in balance between two consecutive days.
    /// It is used to track and summarize the performance of the vault daily.
    /// </remarks>
    public required decimal DailyProfit { get; init; }

    /// <summary>
    /// Gets the percentage profit calculated for a specific vault on a given day.
    /// </summary>
    /// <remarks>
    /// This property represents the daily percentage change in profit for the vault, derived from its balance and profit data.
    /// It is calculated based on the difference in balance over consecutive days.
    /// </remarks>
    public required decimal DailyPercentProfit { get; init; }
}