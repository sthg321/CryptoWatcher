using CryptoWatcher.Infrastructure.Aave.Excel;
using CryptoWatcher.Models;
using CryptoWatcher.UniswapModule.Models;
using SpreadCheetah;

namespace CryptoWatcher.Infrastructure.Uniswap.Excel;

internal class UniswapExcelSheetBuilder : IExcelSheetBuilder
{
    private readonly UniswapDailyReportExcelWorksheetWriter _worksheetWriter;

    public UniswapExcelSheetBuilder(UniswapDailyReportExcelWorksheetWriter worksheetWriter)
    {
        _worksheetWriter = worksheetWriter;
    }

    public bool CanProcess(PlatformDailyReport dailyReport) => dailyReport is UniswapDailyReport;

    public async Task CreateWorksheetAsync(Spreadsheet workbook, PlatformDailyReportData platformDailyReportData,
        CancellationToken ct = default)
    {
        var rowContext = UniswapExcelRowContext.Default.UniswapPoolPositionExcelRow;

        await _worksheetWriter.CreateWorksheetAsync(workbook, platformDailyReportData, rowContext, ct);
    }
}