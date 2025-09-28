using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports;
using CryptoWatcher.Models;
using CryptoWatcher.Shared.Entities;
using SpreadCheetah;
using SpreadCheetah.SourceGeneration;
using SpreadCheetah.Styling;

namespace CryptoWatcher.Infrastructure.Excel;

internal abstract class BaseExcelReportService
{
    private static readonly Dictionary<string, Style> StyleNameToStyleMap = new()
    {
        [ExcelStyleRegistry.Number] =
            new Style { Format = NumberFormat.Standard(StandardNumberFormat.NoDecimalPlaces) },
        [ExcelStyleRegistry.TwoDecimalPlaces] = new Style
            { Format = NumberFormat.Standard(StandardNumberFormat.TwoDecimalPlaces) },
        [ExcelStyleRegistry.Percent] = new Style { Format = NumberFormat.Standard(StandardNumberFormat.Percent) },
    };

    protected static async Task<MemoryStream> CreateExcelWorkbookAsync<TExcelContext, TDailyReport, TDailyReportItem>(
        ExcelSheetDataWriter<TExcelContext, TDailyReport, TDailyReportItem> dataWriter,
        PlatformDailyReportData reportData,
        CancellationToken ct = default) where TDailyReport : PlatformDailyReport
    {
        var ms = new MemoryStream();
        await using var spreadsheet = await Spreadsheet.CreateNewAsync(ms, cancellationToken: ct);

        foreach (var (styleName, style) in StyleNameToStyleMap)
        {
            spreadsheet.AddStyle(style, styleName);
        }

        await dataWriter.CreateWorksheetAsync(spreadsheet, reportData, ct);

        await spreadsheet.FinishAsync(ct);
        ms.Seek(0, SeekOrigin.Begin);

        return ms;
    }

    protected static async Task<MemoryStream> CreateExcelWorkbookAsync(
        Func<Spreadsheet, Task> buildAction,
        CancellationToken ct = default)
    {
        var ms = new MemoryStream();
        await using var spreadsheet = await Spreadsheet.CreateNewAsync(ms, cancellationToken: ct);

        foreach (var (styleName, style) in StyleNameToStyleMap)
        {
            spreadsheet.AddStyle(style, styleName);
        }

        await buildAction(spreadsheet);

        await spreadsheet.FinishAsync(ct);
        ms.Seek(0, SeekOrigin.Begin);

        return ms;
    }

    protected static async Task WriteWalletRow(Spreadsheet workbook, Wallet wallet, CancellationToken ct = default)
    {
        await workbook.AddRowAsync([new DataCell("Кошелек:"), new DataCell(wallet.Address)], ct);
    }

    protected static (DateOnly from, DateOnly to) GetDefaultDatesIfNull(DateOnly? from, DateOnly? to)
    {
        var now = DateTime.Now;
        var monthStart = new DateTime(now.Year, now.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        if (!from.HasValue || !to.HasValue)
        {
            from = DateOnly.FromDateTime(monthStart);
            to = DateOnly.FromDateTime(monthEnd);
        }

        return (from.Value, to.Value);
    }
}