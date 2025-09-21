using CryptoWatcher.HyperliquidModule.Models;
using CryptoWatcher.Infrastructure.Aave.Excel;
using CryptoWatcher.Models;
using SpreadCheetah;

namespace CryptoWatcher.Infrastructure.Hyperliquid.Excel;

internal class HyperliquidExcelSheetBuilder : IExcelSheetBuilder
{
    private readonly HyperliquidDailyReportExcelWorksheetWriter _worksheetWriter;

    public HyperliquidExcelSheetBuilder(HyperliquidDailyReportExcelWorksheetWriter worksheetWriter)
    {
        _worksheetWriter = worksheetWriter;
    }

    public bool CanProcess(PlatformDailyReport dailyReport) => dailyReport is HyperliquidDailyReport;

    public async Task CreateWorksheetAsync(Spreadsheet workbook, PlatformDailyReportData platformDailyReportData,
        CancellationToken ct = default)
    {
        var rowContext = HyperliquidVaultPositionExcelContext.Default.HyperliquidVaultPositionExcelRow;
        await _worksheetWriter.CreateWorksheetAsync(workbook, platformDailyReportData, rowContext, ct);
    }
}