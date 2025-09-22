using CryptoWatcher.Models;
using SpreadCheetah;

namespace CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Abstractions;

/// <summary>
/// Represents an interface for building daily Excel sheets specific to platform report data.
/// Implementations of this interface provide methods to determine if a report can be processed
/// and to create Excel worksheets asynchronously with the provided report data.
/// </summary>
internal interface IDailyExcelSheetBuilder
{
    /// <summary>
    /// Determines whether the specified platform daily report can be processed by the implementing sheet builder.
    /// </summary>
    /// <param name="dailyReport">The platform daily report to evaluate.</param>
    /// <returns>True if the platform daily report can be processed; otherwise, false.</returns>
    bool CanProcess(PlatformDailyReport dailyReport);

    /// <summary>
    /// Creates an Excel worksheet asynchronously using the provided workbook and platform daily report data.
    /// </summary>
    /// <param name="workbook">The workbook to which the worksheet will be added.</param>
    /// <param name="platformDailyReportData">The report data containing platform-specific daily report details.</param>
    /// <param name="ct">The cancellation token to observe during the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateWorksheetAsync(Spreadsheet workbook,
        PlatformDailyReportData platformDailyReportData,
        CancellationToken ct = default);
}