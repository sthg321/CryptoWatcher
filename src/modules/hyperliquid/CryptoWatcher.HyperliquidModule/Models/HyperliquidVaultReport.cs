namespace CryptoWatcher.HyperliquidModule.Models;

public class HyperliquidVaultReport
{
    public required decimal TotalBalance { get; init; }

    public required decimal TotalAbsoluteProfit { get; init; }

    public required decimal TotalPercentProfit { get; init; }

    public required IReadOnlyCollection<HyperliquidVaultReportItem> ReportItems { get; init; } = [];
}