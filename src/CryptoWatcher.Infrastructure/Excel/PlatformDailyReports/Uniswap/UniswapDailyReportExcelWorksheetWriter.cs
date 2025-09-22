using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Uniswap.Mappers;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Uniswap.Models;
using CryptoWatcher.UniswapModule.Models;
using SpreadCheetah;

namespace CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Uniswap;

internal class UniswapDailyReportExcelWorksheetWriter : ExcelSheetDataWriter<UniswapPoolPositionExcelRow,
    UniswapDailyReport, UniswapDailyReportItem>
{
    protected override IReadOnlyCollection<UniswapDailyReportItem> GetReportItems(UniswapDailyReport report) =>
        report.ReportItems;

    protected override async Task WriteRowAsync(Spreadsheet workbook, UniswapDailyReportItem dailyReportItem,
        CancellationToken ct)
    {
        var rowContext = UniswapExcelRowContext.Default.UniswapPoolPositionExcelRow;

        var row = dailyReportItem.MapToExcelRowModel();

        await workbook.AddAsRowAsync(row, rowContext, ct);
    }

    protected override async Task WriteTotalRowAsync(Spreadsheet workbook, UniswapDailyReport dailyReport,
        CancellationToken ct)
    {
        var totalContext = UniswapExcelRowContext.Default.UniswapPoolPositionExcelTotalRow;

        var lastItem = dailyReport.ReportItems.Last();
        var totalRow = dailyReport.MapToExcelModel(TotalName, lastItem.TokenPairSymbols, lastItem.Network);

        await workbook.AddAsRowAsync(totalRow, totalContext, ct);
    }
}