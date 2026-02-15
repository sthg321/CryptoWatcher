using CryptoWatcher.Models;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.ValueObjects;
using SpreadCheetah;
using SpreadCheetah.SourceGeneration;

namespace CryptoWatcher.Infrastructure.Excel.PlatformDailyReports;

public interface IExcelWorksheetWriter
{
    Task CreateWorksheetAsync(
        Spreadsheet spreadsheet,
        PlatformDailyReportData reportData,
        CancellationToken ct = default);
}

public abstract class ExcelSheetDataWriter<TExcelRowContext, TDailyReport, TDailyReportItem> : IExcelWorksheetWriter
    where TDailyReport : PlatformDailyReport
{
    protected const string TotalName = "Итого:";

    public async Task CreateWorksheetAsync(Spreadsheet workbook,
        PlatformDailyReportData platformDailyReportData, CancellationToken ct = default)
    {
        var rowContext = GetWorksheetRow();
        await workbook.StartWorksheetAsync(platformDailyReportData.PlatformName, rowContext, ct);

        await workbook.AddHeaderRowAsync(rowContext, token: ct);

        foreach (var (wallet, positionReport) in platformDailyReportData.Reports)
        {
            await WriteWalletRow(workbook, wallet, ct);

            foreach (var platformDailyReportByNetwork in positionReport.GroupBy(report => report.GetNeworkName()))
            {
                foreach (var platformDailyReport in platformDailyReportByNetwork)
                {
                    var reportItems = GetReportItems((TDailyReport)platformDailyReport);

                    foreach (var dailyReportItem in reportItems)
                    {
                        await WriteRowAsync(workbook, dailyReportItem, ct);   
                    }
                    
                    await WriteTotalRowAsync(workbook, (TDailyReport)platformDailyReport, ct);
                    
                    await workbook.AddRowAsync([], ct);
                }

                await workbook.AddRowAsync([], ct);
            }
        }
    }

    protected abstract WorksheetRowTypeInfo<TExcelRowContext> GetWorksheetRow();

    protected abstract IReadOnlyCollection<TDailyReportItem> GetReportItems(TDailyReport report);

    protected abstract Task WriteRowAsync(Spreadsheet workbook, TDailyReportItem dailyReportItem,
        CancellationToken ct);

    protected abstract Task WriteTotalRowAsync(Spreadsheet workbook, TDailyReport dailyReport, CancellationToken ct);

    private static async Task WriteWalletRow(Spreadsheet workbook, EvmAddress wallet, CancellationToken ct = default)
    {
        await workbook.AddRowAsync([new DataCell("Кошелек:"), new DataCell(wallet)], ct);
    }
}