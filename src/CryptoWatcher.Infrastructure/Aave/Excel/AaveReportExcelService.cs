using CryptoWatcher.AaveModule.Extensions;
using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Infrastructure.Aave.ExcelModels;
using CryptoWatcher.Infrastructure.Excel;
using CryptoWatcher.Models;
using CryptoWatcher.Shared.Entities;
using Microsoft.Extensions.DependencyInjection;
using SpreadCheetah;
using SpreadCheetah.SourceGeneration;

namespace CryptoWatcher.Infrastructure.Aave.Excel;

public interface IAaveReportExcelService
{
    Task<Stream> CreateReportAsync(IReadOnlyCollection<Wallet> wallets, DateOnly? from, DateOnly? to,
        CancellationToken ct = default);
}

internal interface IExcelSheetBuilder
{
    bool CanProcess(PlatformDailyReport dailyReport);

    Task CreateWorksheetAsync(Spreadsheet workbook,
        PlatformDailyReportData platformDailyReportData,
        CancellationToken ct = default);
}

internal class AaveReportExcelService : BaseExcelReportService, IAaveReportExcelService
{
    private readonly IPlatformDailyReportDataProvider _platformDailyReportDataProvider;
    private readonly AaveDailyReportExcelWorksheetWriter _worksheetWriter;

    public AaveReportExcelService(
        [FromKeyedServices(AaveModuleKeyedService.DailyPlatformKeyService)]
        IPlatformDailyReportDataProvider platformDailyReportDataProvider,
        AaveDailyReportExcelWorksheetWriter worksheetWriter)
    {
        _platformDailyReportDataProvider = platformDailyReportDataProvider;
        _worksheetWriter = worksheetWriter;
    }

    public async Task<Stream> CreateReportAsync(IReadOnlyCollection<Wallet> wallets, DateOnly? from, DateOnly? to,
        CancellationToken ct = default)
    {
        var (fromDate, toDate) = GetDefaultDatesIfNull(from, to);

        var reportData = await _platformDailyReportDataProvider.GetReportDataAsync(wallets, fromDate, toDate, ct);

        var rowContext = AaveExcelReportContext.Default.AavePositionExcelRow;
        var ms = await CreateExcelWorkbookAsync(reportData.PlatformName, _worksheetWriter, rowContext, reportData, ct);

        return ms;
    }
}

[WorksheetRow(typeof(AavePositionExcelRow))]
[WorksheetRow(typeof(AavePositionExcelTotalRow))]
internal partial class AaveExcelReportContext : WorksheetRowContext;