using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Abstractions;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Total;
using CryptoWatcher.Models;

namespace CryptoWatcher.Infrastructure.Excel.PlatformDailyReports;

internal class DailySummaryReportBuilder : IDailySummaryReportBuilder
{
    private readonly IDailyExcelSheetBuilder[] _spreadCheetahSheetBuilders;
    private readonly IDailyTotalReportWorksheetBuilder _dailyTotalReportWorksheetBuilder;
    private readonly IExcelReportGenerator _excelReportGenerator;

    public DailySummaryReportBuilder(IEnumerable<IDailyExcelSheetBuilder> spreadCheetahSheetBuilders,
        IDailyTotalReportWorksheetBuilder dailyTotalReportWorksheetBuilder, IExcelReportGenerator excelReportGenerator)
    {
        _dailyTotalReportWorksheetBuilder = dailyTotalReportWorksheetBuilder;
        _excelReportGenerator = excelReportGenerator;
        _spreadCheetahSheetBuilders = spreadCheetahSheetBuilders.ToArray();
    }

    public async Task<Stream> BuildReportAsync(IReadOnlyCollection<PlatformDailyReportData> reportsByPlatform,
        CancellationToken ct = default)
    {
        var result = await _excelReportGenerator.CreateExcelWorkbookAsync(async workbook =>
        {
            await _dailyTotalReportWorksheetBuilder.CreateTotalWorksheetAsync(workbook, reportsByPlatform, ct);

            foreach (var reportByPlatform in reportsByPlatform)
            {
                if (reportByPlatform.Reports.Count == 0)
                {
                    continue;
                }

                var report = reportByPlatform.Reports.First();
                if (report.Value.Count == 0)
                {
                    continue;
                }

                var reportItem = report.Value.First();

                var sheetBuilder = _spreadCheetahSheetBuilders.FirstOrDefault(builder =>
                    builder.CanProcess(reportItem));

                if (sheetBuilder is null)
                {
                    throw new InvalidOperationException(
                        $"For type: {reportItem.GetType().Name}  sheet builder was not found");
                }

                await sheetBuilder.CreateWorksheetAsync(workbook, reportByPlatform, ct);
            }
        }, ct);

        return result;
    }
}