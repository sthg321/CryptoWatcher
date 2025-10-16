using CryptoWatcher.Modules.Aave.Models;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Aave.Mappers;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Aave.Models;
using SpreadCheetah;
using SpreadCheetah.SourceGeneration;

namespace CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Aave;

internal class AaveDailyReportExcelWorksheetWriter : ExcelSheetDataWriter<AavePositionExcelRow, AaveDailyReport,
    AaveDailyReportItem>
{
    protected override WorksheetRowTypeInfo<AavePositionExcelRow> GetWorksheetRow() =>
        AaveExcelReportContext.Default.AavePositionExcelRow;

    protected override IReadOnlyCollection<AaveDailyReportItem> GetReportItems(AaveDailyReport report) =>
        report.ReportItems;

    protected override async Task WriteRowAsync(Spreadsheet workbook, AaveDailyReportItem dailyReportItem,
        CancellationToken ct)
    {
        var row = dailyReportItem.MapToExcelRow();
        await workbook.AddAsRowAsync(row, GetWorksheetRow(), ct);
    }

    protected override async Task WriteTotalRowAsync(Spreadsheet workbook, AaveDailyReport dailyReport,
        CancellationToken ct)
    {
        var totalRow = dailyReport.MapToExcelModel(TotalName);

        await workbook.AddAsRowAsync(totalRow, AaveExcelReportContext.Default.AavePositionExcelTotalRow,
            ct);
    }
}