using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Hyperliquid.Mappers;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Hyperliquid.Models;
using CryptoWatcher.Modules.Hyperliquid.Models;
using SpreadCheetah;
using SpreadCheetah.SourceGeneration;

namespace CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Hyperliquid;

internal class HyperliquidDailyReportExcelWorksheetWriter : ExcelSheetDataWriter<HyperliquidVaultPositionExcelRow,
    HyperliquidDailyReport, HyperliquidVaultReportItem>
{
    protected override WorksheetRowTypeInfo<HyperliquidVaultPositionExcelRow> GetWorksheetRow() =>
        HyperliquidVaultPositionExcelContext.Default.HyperliquidVaultPositionExcelRow;

    protected override IReadOnlyCollection<HyperliquidVaultReportItem> GetReportItems(HyperliquidDailyReport report) =>
        report.ReportItems;

    protected override async Task WriteRowAsync(Spreadsheet workbook, HyperliquidVaultReportItem dailyReportItem,
        CancellationToken ct)
    {
        var rowContext = HyperliquidVaultPositionExcelContext.Default.HyperliquidVaultPositionExcelRow;

        var row = dailyReportItem.MapToExcelModel();

        await workbook.AddAsRowAsync(row, rowContext, ct);
    }

    protected override async Task WriteTotalRowAsync(Spreadsheet workbook, HyperliquidDailyReport dailyReport,
        CancellationToken ct)
    {
        var rowContext = HyperliquidVaultPositionExcelContext.Default.HyperliquidVaultPositionExcelTotalRow;

        var totalRow = dailyReport.MapToExcelModel(TotalName, "-");

        await workbook.AddAsRowAsync(totalRow, rowContext, ct);
    }
}