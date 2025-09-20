using CryptoWatcher.Models;

namespace CryptoWatcher.Abstractions;

public interface IDailySummaryReportBuilder
{
    Task<Stream> BuildReportAsync(IReadOnlyCollection<PlatformDailyReportData> reportsByPlatform,
        CancellationToken ct = default);
}