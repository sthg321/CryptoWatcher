using CryptoWatcher.Models;

namespace CryptoWatcher.Modules.Aave.Models;

/// <summary>
/// Represents a report for a position in Aave, including balance, token details, and daily-profit metrics.
/// </summary>
public class AaveDailyReport : PlatformDailyReport
{
    public string TokenSymbol => ReportItems.FirstOrDefault()?.TokenSymbol ?? string.Empty;

    public required decimal PositionGrowInUsd { get; init; }

    public required decimal PositionInToken { get; init; }

    public required decimal ProfitInToken { get; init; }

    public IReadOnlyCollection<AaveDailyReportItem> ReportItems { get; init; } = [];
}