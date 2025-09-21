using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Models;
using CryptoWatcher.Shared.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoWatcher.Application.Reports;

public class DailySummaryReportProvider : IDailySummaryReportProvider
{
    private readonly IEnumerable<IPlatformDailyReportDataProvider> _platformReportProviders;
    private readonly IDailySummaryReportBuilder _dailySummaryReportBuilder;

    public DailySummaryReportProvider(IServiceProvider serviceProvider,
        IDailySummaryReportBuilder dailySummaryReportBuilder)
    {
        _platformReportProviders = serviceProvider.GetServices<IPlatformDailyReportDataProvider>();
        _dailySummaryReportBuilder = dailySummaryReportBuilder;
    }

    public async Task<Stream> CreateDailySummaryReportAsync(IReadOnlyCollection<Wallet> wallets, DateOnly from, DateOnly to,
        CancellationToken ct = default)
    {
        var reports = new List<PlatformDailyReportData>();
        foreach (var dailyPlatformReportProvider in _platformReportProviders)
        {
            var platformDailyReport = await dailyPlatformReportProvider.GetReportDataAsync(wallets, from, to, ct);
            reports.Add(platformDailyReport);
        }

        var result = await _dailySummaryReportBuilder.BuildReportAsync(reports, ct);

        return result;
    }
}