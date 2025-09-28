using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.HyperliquidModule.Extensions;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Hyperliquid.Models;
using CryptoWatcher.Shared.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Hyperliquid;

/// <summary>
/// Defines methods for generating Excel reports based on Hyperliquid data.
/// </summary>
public interface IHyperliquidExcelService
{
    /// <summary>
    /// Generates a report based on the specified date range and returns it as a stream.
    /// </summary>
    /// <param name="wallets"></param>
    /// <param name="from">The starting date for the report. If null, the default start date is the first day of the current month.</param>
    /// <param name="to">The ending date for the report. If null, the default end date is the last day of the current month.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A stream containing the generated report in Excel format.</returns>
    Task<Stream> CreateReportAsync(IReadOnlyCollection<Wallet> wallets, DateOnly? from, DateOnly? to,
        CancellationToken ct = default);
}

internal class HyperliquidExcelService : BaseExcelReportService, IHyperliquidExcelService
{
    private readonly IPlatformDailyReportDataProvider _platformDailyReportDataProvider;
    private readonly HyperliquidDailyReportExcelWorksheetWriter _worksheetWriter;

    public HyperliquidExcelService(
        [FromKeyedServices(HyperliquidModuleKeyedService.DailyPlatformKeyService)]
        IPlatformDailyReportDataProvider platformDailyReportDataProvider,
        HyperliquidDailyReportExcelWorksheetWriter worksheetWriter)
    {
        _platformDailyReportDataProvider = platformDailyReportDataProvider;
        _worksheetWriter = worksheetWriter;
    }

    public async Task<Stream> CreateReportAsync(IReadOnlyCollection<Wallet> wallets, DateOnly? from, DateOnly? to,
        CancellationToken ct = default)
    {
        var (fromDate, toDate) = GetDefaultDatesIfNull(from, to);

        var platformDailyReports =
            await _platformDailyReportDataProvider.GetReportDataAsync(wallets, fromDate, toDate, ct);

        var rowContext = HyperliquidVaultPositionExcelContext.Default.HyperliquidVaultPositionExcelRow;
        
        return await CreateExcelWorkbookAsync(_worksheetWriter,
            platformDailyReports, ct);
    }
}