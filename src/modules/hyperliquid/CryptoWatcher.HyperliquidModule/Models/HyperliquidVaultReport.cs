namespace CryptoWatcher.HyperliquidModule.Models;

/// <summary>
/// Represents a report of a Hyperliquid vault, including financial metrics and detailed report items.
/// </summary>
public class HyperliquidVaultReport
{
    /// <summary>
    /// Gets the total balance amount for the Hyperliquid vault, capturing the latest financial state.
    /// </summary>
    public required decimal TotalBalance { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public required decimal TotalAbsoluteProfit { get; init; }

    /// <summary>
    /// Represents the total percentage profit for a Hyperliquid vault.
    /// This value is calculated based on the relative-profit percentage
    /// derived from the vault's performance within a specific time period.
    /// </summary>
    public required decimal TotalPercentProfit { get; init; }

    /// <summary>
    /// Gets the collection of detailed report items for the Hyperliquid vault,
    /// providing snapshot information such as balance and profits for each day.
    /// </summary>
    public required IReadOnlyCollection<HyperliquidVaultReportItem> ReportItems { get; init; } = [];
}