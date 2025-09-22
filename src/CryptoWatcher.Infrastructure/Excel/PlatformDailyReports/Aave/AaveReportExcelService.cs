using CryptoWatcher.AaveModule.Extensions;
using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Aave.Models;
using CryptoWatcher.Shared.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Aave;

/// <summary>
/// Provides functionality to generate daily report Excel files for the Aave platform.
/// This service handles the creation of an Excel report containing data relevant to the specified wallets
/// and time period.
/// </summary>
public interface IAaveDailyReportExcelService
{
    /// <summary>
    /// Generates an Excel report for Aave platform based on wallet data and date range.
    /// </summary>
    /// <param name="wallets">A collection of wallets to include in the report.</param>
    /// <param name="from">The start date for the report data range. If null, no start date filter is applied.</param>
    /// <param name="to">The end date for the report data range. If null, no end date filter is applied.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is a stream containing the generated Excel report.</returns>
    Task<Stream> CreateReportAsync(IReadOnlyCollection<Wallet> wallets, DateOnly? from, DateOnly? to,
        CancellationToken ct = default);
}

internal class AaveDailyReportExcelService : BaseExcelReportService, IAaveDailyReportExcelService
{
    private readonly IPlatformDailyReportDataProvider _platformDailyReportDataProvider;
    private readonly AaveDailyReportExcelWorksheetWriter _worksheetWriter;

    public AaveDailyReportExcelService(
        [FromKeyedServices(AaveModuleKeyedService.DailyPlatformKeyService)]
        IPlatformDailyReportDataProvider platformDailyReportDataProvider,
        AaveDailyReportExcelWorksheetWriter worksheetWriter)
    {
        _platformDailyReportDataProvider = platformDailyReportDataProvider;
        _worksheetWriter = worksheetWriter;
    }

    public async Task<Stream> CreateReportAsync(IReadOnlyCollection<Wallet> wallets, DateOnly? from, DateOnly? to,
        CancellationToken ct = default)
    {
        var (fromDate, toDate) = GetDefaultDatesIfNull(from, to);

        var reportData = await _platformDailyReportDataProvider.GetReportDataAsync(wallets, fromDate, toDate, ct);

        var rowContext = AaveExcelReportContext.Default.AavePositionExcelRow;
        var ms = await CreateExcelWorkbookAsync(_worksheetWriter, rowContext, reportData, ct);

        return ms;
    }
}