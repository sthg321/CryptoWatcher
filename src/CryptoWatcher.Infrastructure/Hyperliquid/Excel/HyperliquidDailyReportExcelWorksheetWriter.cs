using CryptoWatcher.HyperliquidModule.Models;
using CryptoWatcher.Infrastructure.Hyperliquid.ExcelModels;
using CryptoWatcher.Infrastructure.Hyperliquid.Mappers;
using SpreadCheetah;

namespace CryptoWatcher.Infrastructure.Hyperliquid.Excel;

internal class HyperliquidDailyReportExcelWorksheetWriter : ExcelSheetDataWriter<HyperliquidVaultPositionExcelRow,
    HyperliquidDailyReport, HyperliquidVaultReportItem>
{
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