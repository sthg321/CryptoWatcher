using CryptoWatcher.HyperliquidModule.Models;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Abstractions;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Hyperliquid.Models;
using CryptoWatcher.Models;
using SpreadCheetah;

namespace CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Hyperliquid;

internal class HyperliquidDailyExcelSheetBuilder : IDailyExcelSheetBuilder
{
    private readonly HyperliquidDailyReportExcelWorksheetWriter _worksheetWriter;

    public HyperliquidDailyExcelSheetBuilder(HyperliquidDailyReportExcelWorksheetWriter worksheetWriter)
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