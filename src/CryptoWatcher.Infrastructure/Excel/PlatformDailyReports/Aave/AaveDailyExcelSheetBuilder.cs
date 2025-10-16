using CryptoWatcher.Modules.Aave.Models;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Abstractions;
using CryptoWatcher.Models;
using SpreadCheetah;

namespace CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Aave;

internal class AaveDailyExcelSheetBuilder : IDailyExcelSheetBuilder
{
    private readonly AaveDailyReportExcelWorksheetWriter _worksheetWriter;

    public AaveDailyExcelSheetBuilder(AaveDailyReportExcelWorksheetWriter worksheetWriter)
    {
        _worksheetWriter = worksheetWriter;
    }

    public bool CanProcess(PlatformDailyReport dailyReport) => dailyReport is AaveDailyReport;

    public async Task CreateWorksheetAsync(Spreadsheet workbook, PlatformDailyReportData platformDailyReportData,
        CancellationToken ct = default)
    {
        await _worksheetWriter.CreateWorksheetAsync(workbook, platformDailyReportData, ct);
    }
}