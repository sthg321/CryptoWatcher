using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Models;
using CryptoWatcher.Shared.Entities;
using SpreadCheetah;
using SpreadCheetah.Styling;

namespace CryptoWatcher.Infrastructure.Excel.PlatformDailyReports;

internal interface IExcelReportGenerator
{
    /// <summary>
    /// Creates a platform daily report in Excel format
    /// </summary>
    Task<ExcelReport> CreatePlatformDailyReportAsync(
        IPlatformDailyReportDataProvider dataProvider,
        IExcelWorksheetWriter worksheetWriter,
        IReadOnlyCollection<Wallet> wallets,
        DateOnly? from,
        DateOnly? to,
        string reportName,
        CancellationToken ct = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="buildAction"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<MemoryStream> CreateExcelWorkbookAsync(
        Func<Spreadsheet, Task> buildAction,
        CancellationToken ct = default);
}

internal class ExcelReportGenerator : IExcelReportGenerator
{
    private static readonly Dictionary<string, Style> StyleNameToStyleMap = new()
    {
        [ExcelStyleRegistry.Number] =
            new Style { Format = NumberFormat.Standard(StandardNumberFormat.NoDecimalPlaces) },
        [ExcelStyleRegistry.TwoDecimalPlaces] = new Style
            { Format = NumberFormat.Standard(StandardNumberFormat.TwoDecimalPlaces) },
        [ExcelStyleRegistry.Percent] = new Style { Format = NumberFormat.Standard(StandardNumberFormat.Percent) },
    };

    public async Task<ExcelReport> CreatePlatformDailyReportAsync(
        IPlatformDailyReportDataProvider dataProvider,
        IExcelWorksheetWriter worksheetWriter,
        IReadOnlyCollection<Wallet> wallets,
        DateOnly? from,
        DateOnly? to,
        string reportName,
        CancellationToken ct = default)
    {
        var (fromDate, toDate) = GetDefaultDatesIfNull(from, to);
        var reportData = await dataProvider.GetReportDataAsync(wallets, fromDate, toDate, ct);

        var stream = await CreateExcelWorkbookAsync(worksheetWriter, reportData, ct);

        var fileName = GenerateFileName(reportName, fromDate, toDate);
        return new ExcelReport(fileName, stream);
    }

    private static async Task<MemoryStream> CreateExcelWorkbookAsync(
        IExcelWorksheetWriter worksheetWriter,
        PlatformDailyReportData reportData,
        CancellationToken ct = default)
    {
        var ms = new MemoryStream();
        await using var spreadsheet = await Spreadsheet.CreateNewAsync(ms, cancellationToken: ct);

        RegisterStyles(spreadsheet);
        await worksheetWriter.CreateWorksheetAsync(spreadsheet, reportData, ct);

        await spreadsheet.FinishAsync(ct);
        ms.Seek(0, SeekOrigin.Begin);

        return ms;
    }

    public async Task<MemoryStream> CreateExcelWorkbookAsync(
        Func<Spreadsheet, Task> buildAction,
        CancellationToken ct = default)
    {
        var ms = new MemoryStream();
        await using var spreadsheet = await Spreadsheet.CreateNewAsync(ms, cancellationToken: ct);

        RegisterStyles(spreadsheet);

        await buildAction(spreadsheet);

        await spreadsheet.FinishAsync(ct);
        ms.Seek(0, SeekOrigin.Begin);

        return ms;
    }
    
    private static void RegisterStyles(Spreadsheet spreadsheet)
    {
        foreach (var (styleName, style) in StyleNameToStyleMap)
        {
            spreadsheet.AddStyle(style, styleName);
        }
    }

    private static (DateOnly from, DateOnly to) GetDefaultDatesIfNull(DateOnly? from, DateOnly? to)
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

    private static string GenerateFileName(string reportName, DateOnly fromDate, DateOnly toDate)
    {
        var fromStr = fromDate.ToString("yyyyMMdd");
        var toStr = toDate.ToString("yyyyMMdd");

        return fromStr == toStr
            ? $"{reportName}_{fromStr}.xlsx"
            : $"{reportName}_{fromStr}_{toStr}.xlsx";
    }
}