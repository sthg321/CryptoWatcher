using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Uniswap.Mappers;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Uniswap.Models;
using CryptoWatcher.Modules.Uniswap.Models;
using SpreadCheetah;
using SpreadCheetah.SourceGeneration;

namespace CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Uniswap;

internal class UniswapDailyReportExcelWorksheetWriter : ExcelSheetDataWriter<UniswapPoolPositionExcelRow,
    UniswapDailyReport, UniswapDailyReportItem>
{
    protected override WorksheetRowTypeInfo<UniswapPoolPositionExcelRow> GetWorksheetRow() =>
        UniswapExcelRowContext.Default.UniswapPoolPositionExcelRow;

    protected override IReadOnlyCollection<UniswapDailyReportItem> GetReportItems(UniswapDailyReport report) =>
        report.ReportItems.GroupBy(item => item.Network).SelectMany(items => items.ToArray()).ToArray();

    protected override async Task WriteRowAsync(Spreadsheet workbook, UniswapDailyReportItem dailyReportItem,
        CancellationToken ct)
    {
        var row = dailyReportItem.MapToExcelRowModel();

        await workbook.AddAsRowAsync(row, GetWorksheetRow(), ct);
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