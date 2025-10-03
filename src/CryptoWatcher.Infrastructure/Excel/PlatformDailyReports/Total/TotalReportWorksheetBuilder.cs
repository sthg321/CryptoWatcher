using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Total.Models;
using CryptoWatcher.Models;
using CryptoWatcher.Shared.ValueObjects;
using SpreadCheetah;

namespace CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Total;

/// <summary>
/// Defines the interface for building a daily total report worksheet in a spreadsheet.
/// </summary>
internal interface IDailyTotalReportWorksheetBuilder
{
    /// <summary>
    /// Creates a total worksheet in the given spreadsheet based on the provided platform daily reports.
    /// </summary>
    /// <param name="spreadsheet">The spreadsheet where the total worksheet will be created.</param>
    /// <param name="platformDailyReports">The collection of platform daily reports used to populate the worksheet.</param>
    /// <param name="ct">The cancellation token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateTotalWorksheetAsync(Spreadsheet spreadsheet,
        IReadOnlyCollection<PlatformDailyReportData> platformDailyReports,
        CancellationToken ct = default);
}

internal class DailyTotalReportWorksheetBuilder : IDailyTotalReportWorksheetBuilder
{
    private const string TotalReportSheetName = "Все платформы";
    
    public async Task CreateTotalWorksheetAsync(Spreadsheet spreadsheet,
        IReadOnlyCollection<PlatformDailyReportData> platformDailyReports,
        CancellationToken ct = default)
    {
        var rowContext = TotalReportExcelRowContext.Default.TotalPlatformDailyReportExcelRow;
        await spreadsheet.StartWorksheetAsync(TotalReportSheetName, rowContext, ct);

        await spreadsheet.AddHeaderRowAsync(rowContext, token: ct);

        Money initialPositionInUsd = 0;
        Money currentPositionInUsd = 0;
        foreach (var platformDailyReportData in platformDailyReports)
        {
            var platformTotalRow = platformDailyReportData.Reports.Values.Select(pair =>
                    new TotalPlatformDailyReportExcelRow
                    {
                        PlatformName = platformDailyReportData.PlatformName,
                        InitialPositionInUsd = pair.Sum(report => report.PositionInUsd - report.ProfitInUsd),
                        CurrentPositionInUsd = pair.Sum(report => report.PositionInUsd)
                    })
                .First();

            await spreadsheet.AddAsRowAsync(platformTotalRow, rowContext, ct);

            initialPositionInUsd += platformTotalRow.InitialPositionInUsd;
            currentPositionInUsd += platformTotalRow.CurrentPositionInUsd;
        }

        var totalRow = new TotalPlatformDailyReportExcelTotalRow
        {
            TotalName = "Итого",
            InitialPositionInUsd = initialPositionInUsd,
            CurrentPositionInUsd = currentPositionInUsd
        };

        var totalRowContext = TotalReportExcelRowContext.Default.TotalPlatformDailyReportExcelTotalRow;
        await spreadsheet.AddAsRowAsync(totalRow, totalRowContext, ct);
    }
}