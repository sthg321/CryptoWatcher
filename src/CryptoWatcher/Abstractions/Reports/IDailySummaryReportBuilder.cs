using CryptoWatcher.Models;

namespace CryptoWatcher.Abstractions.Reports;

public interface IDailySummaryReportBuilder
{
    Task<Stream> BuildReportAsync(IReadOnlyCollection<PlatformDailyReportData> reportsByPlatform,
        CancellationToken ct = default);
}