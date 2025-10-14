using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Abstractions;
using CryptoWatcher.Models;
using CryptoWatcher.Modules.Uniswap.Models;
using SpreadCheetah;

namespace CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Uniswap;

internal class UniswapDailyExcelSheetBuilder : IDailyExcelSheetBuilder
{
    private readonly UniswapDailyReportExcelWorksheetWriter _worksheetWriter;

    public UniswapDailyExcelSheetBuilder(UniswapDailyReportExcelWorksheetWriter worksheetWriter)
    {
        _worksheetWriter = worksheetWriter;
    }

    public bool CanProcess(PlatformDailyReport dailyReport) => dailyReport is UniswapDailyReport;

    public async Task CreateWorksheetAsync(Spreadsheet workbook, PlatformDailyReportData platformDailyReportData,
        CancellationToken ct = default)
    {
        await _worksheetWriter.CreateWorksheetAsync(workbook, platformDailyReportData, ct);
    }
}