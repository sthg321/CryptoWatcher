using CryptoWatcher.Abstractions;
using CryptoWatcher.Infrastructure.Aave;
using CryptoWatcher.Infrastructure.Excel;
using CryptoWatcher.Models;

namespace CryptoWatcher.Infrastructure.Reports;

internal class DailySummaryReportBuilder : BaseExcelReportService, IDailySummaryReportBuilder
{
    private readonly IExcelSheetBuilder[] _spreadCheetahSheetBuilders;

    public DailySummaryReportBuilder(IEnumerable<IExcelSheetBuilder> spreadCheetahSheetBuilders)
    {
        _spreadCheetahSheetBuilders = spreadCheetahSheetBuilders.ToArray();
    }

    public async Task<Stream> BuildReportAsync(IReadOnlyCollection<PlatformDailyReportData> reportsByPlatform,
        CancellationToken ct = default)
    {
        var result = await CreateExcelWorkbookAsync(async workbook =>
        {
            foreach (var reportByPlatform in reportsByPlatform)
            {
                foreach (var (wallet, reportData) in reportByPlatform.Reports)
                {
                    await workbook.StartWorksheetAsync(reportByPlatform.PlatformName, token: ct);

                    if (reportData.Count == 0)
                    {
                        continue;
                    }

                    var reportItem = reportData[0];
                    
                    var sheetBuilder = _spreadCheetahSheetBuilders.FirstOrDefault(builder =>
                        builder.CanProcess(reportItem));
                    
                    if (sheetBuilder is null)
                    {
                        throw new InvalidOperationException(
                            $"For type: {reportData.GetType().Name}  sheet builder was not found");
                    }

                    await sheetBuilder.CreateHeaderAsync(workbook, wallet, ct);
                    
                    foreach (var platformDailyReport in reportData)
                    {
                        await sheetBuilder.WriteDataToWorksheetAsync(workbook, platformDailyReport, ct);
                    }

                    await workbook.AddRowAsync([], ct);

                }
            }
        }, ct);

        return result;
    }
}