using CryptoWatcher.AaveModule.Models;
using CryptoWatcher.Infrastructure.Aave.ExcelModels;
using CryptoWatcher.Infrastructure.Aave.Mappers;
using SpreadCheetah;

namespace CryptoWatcher.Infrastructure.Aave.Excel;

internal class AaveDailyReportExcelWorksheetWriter : ExcelSheetDataWriter<AavePositionExcelRow, AaveDailyReport,
    AaveDailyReportItem>
{
    protected override IReadOnlyCollection<AaveDailyReportItem> GetReportItems(AaveDailyReport report) =>
        report.ReportItems;

    protected override async Task WriteRowAsync(Spreadsheet workbook, AaveDailyReportItem dailyReportItem,
        CancellationToken ct)
    {
        var row = dailyReportItem.MapToExcelRow();
        await workbook.AddAsRowAsync(row, AaveExcelReportContext.Default.AavePositionExcelRow, ct);
    }

    protected override async Task WriteTotalRowAsync(Spreadsheet workbook, AaveDailyReport dailyReport,
        CancellationToken ct)
    {
        var totalRow = dailyReport.MapToExcelModel(TotalName);

        await workbook.AddAsRowAsync(totalRow, AaveExcelReportContext.Default.AavePositionExcelTotalRow,
            ct);
    }
}