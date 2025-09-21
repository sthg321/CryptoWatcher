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

    protected const string TotalName = "Итого:";

    protected static async Task<MemoryStream> CreateExcelWorkbookAsync<TRow>(
        string sheetName,
        WorksheetRowTypeInfo<TRow> rowContext,
        Func<Spreadsheet, Task> buildAction,
        CancellationToken ct = default)
    {
        var ms = new MemoryStream();
        await using var spreadsheet = await Spreadsheet.CreateNewAsync(ms, cancellationToken: ct);

        foreach (var (styleName, style) in StyleNameToStyleMap)
        {
            spreadsheet.AddStyle(style, styleName);
        }

        await spreadsheet.StartWorksheetAsync(sheetName, rowContext, ct);

        await spreadsheet.AddHeaderRowAsync(rowContext, token: ct);

        await buildAction(spreadsheet);

        await spreadsheet.FinishAsync(ct);
        ms.Seek(0, SeekOrigin.Begin);

        return ms;
    }

    protected static async Task<MemoryStream> CreateExcelWorkbookAsync<TExcelContext, TDailyReport, TDailyReportItem>(
        string sheetName,
        ExcelSheetDataWriter<TExcelContext, TDailyReport, TDailyReportItem> dataWriter,
        WorksheetRowTypeInfo<TExcelContext> rowContext,
        PlatformDailyReportData reportData,
        CancellationToken ct = default) where TDailyReport : PlatformDailyReport
    {
        var ms = new MemoryStream();
        await using var spreadsheet = await Spreadsheet.CreateNewAsync(ms, cancellationToken: ct);

        foreach (var (styleName, style) in StyleNameToStyleMap)
        {
            spreadsheet.AddStyle(style, styleName);
        }

        await dataWriter.CreateWorksheetAsync(spreadsheet, reportData, rowContext, ct);

        await spreadsheet.FinishAsync(ct);
        ms.Seek(0, SeekOrigin.Begin);

        return ms;
    }

    protected static async Task StartWorksheetAsync<TRow>(
        string sheetName,
        WorksheetRowTypeInfo<TRow> rowContext,
        Spreadsheet spreadsheet,
        CancellationToken ct = default)
    {
        await spreadsheet.StartWorksheetAsync(sheetName, rowContext, ct);

        await spreadsheet.AddHeaderRowAsync(rowContext, token: ct);
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