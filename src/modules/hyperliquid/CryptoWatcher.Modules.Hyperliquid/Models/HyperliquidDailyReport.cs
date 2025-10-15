using CryptoWatcher.Models;

namespace CryptoWatcher.Modules.Hyperliquid.Models;

/// <summary>
/// Represents a report of a Hyperliquid vault, including financial metrics and detailed report items.
/// </summary>
public class HyperliquidDailyReport : PlatformDailyReport
{
    /// <summary>
    /// Gets the collection of detailed report items for the Hyperliquid vault,
    /// providing snapshot information such as balance and profits for each day.
    /// </summary>
    public required IReadOnlyCollection<HyperliquidVaultReportItem> ReportItems { get; init; } = [];
}