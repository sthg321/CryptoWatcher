using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Uniswap.Models;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.UniswapModule.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Uniswap;

/// <summary>
/// Interface defining the contract for generating Uniswap daily reports in Excel format.
/// This service provides functionality to create an asynchronous report for specified wallets
/// over a given date range, producing a stream of the resulting Excel file.
/// </summary>
public interface IUniswapDailyExcelReportService
{
    /// <summary>
    /// Asynchronously generates an Excel report for the specified wallets containing Uniswap platform daily metrics,
    /// within the given date range, and returns the resulting file stream.
    /// </summary>
    /// <param name="wallets">A collection of wallets for which to generate the report.</param>
    /// <param name="from">The starting date for the report (optional).</param>
    /// <param name="to">The ending date for the report (optional).</param>
    /// <param name="ct">A cancellation token to cancel the report generation operation (optional).</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a stream of the generated report.</returns>
    Task<Stream> CreateReportAsync(IReadOnlyCollection<Wallet> wallets, DateOnly? from, DateOnly? to,
        CancellationToken ct = default);
}

/// <summary>
/// <see cref="IUniswapDailyExcelReportService"/>
/// </summary>
internal class UniswapDailyExcelReportService : BaseExcelReportService, IUniswapDailyExcelReportService
{
    private readonly IPlatformDailyReportDataProvider _dailyReportDataProvider;
    private readonly UniswapDailyReportExcelWorksheetWriter _worksheetWriter;

    public UniswapDailyExcelReportService(
        [FromKeyedServices(UniswapModuleKeyedService.DailyPlatformKeyService)]
        IPlatformDailyReportDataProvider dailyReportDataProvider,
        UniswapDailyReportExcelWorksheetWriter worksheetWriter)
    {
        _dailyReportDataProvider = dailyReportDataProvider;
        _worksheetWriter = worksheetWriter;
    }

    public async Task<Stream> CreateReportAsync(
        IReadOnlyCollection<Wallet> wallets,
        DateOnly? from, DateOnly? to, CancellationToken ct = default)
    {
        var (fromDate, toDate) = GetDefaultDatesIfNull(from, to);

        var dailyReportData = await _dailyReportDataProvider.GetReportDataAsync(wallets, fromDate, toDate, ct);

        var rowContext = UniswapExcelRowContext.Default.UniswapPoolPositionExcelRow;

        return await CreateExcelWorkbookAsync(_worksheetWriter,
            dailyReportData, ct);
    }
}