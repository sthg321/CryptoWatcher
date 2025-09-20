using CryptoWatcher.AaveModule.Extensions;
using CryptoWatcher.AaveModule.Models;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Infrastructure.Aave.ExcelModels;
using CryptoWatcher.Infrastructure.Aave.Mappers;
using CryptoWatcher.Infrastructure.Excel;
using CryptoWatcher.Models;
using CryptoWatcher.Shared.Entities;
using Microsoft.Extensions.DependencyInjection;
using SpreadCheetah;
using SpreadCheetah.SourceGeneration;

namespace CryptoWatcher.Infrastructure.Aave;

public interface IAaveReportExcelService
{
    Task<Stream> CreateReportAsync(IReadOnlyCollection<Wallet> wallets, DateOnly? from, DateOnly? to,
        CancellationToken ct = default);
}

internal interface IExcelSheetBuilder
{
    bool CanProcess(PlatformDailyReport dailyReport);

    Task CreateHeaderAsync(Spreadsheet workbook, Wallet wallet, CancellationToken ct = default);

    Task WriteDataToWorksheetAsync(Spreadsheet workbook,
        PlatformDailyReport dailyReport,
        CancellationToken ct = default);
}

internal class AaveReportExcelService : BaseExcelReportService, IAaveReportExcelService, IExcelSheetBuilder
{
    private readonly IPlatformDailyReportDataProvider _platformDailyReportDataProvider;

    public AaveReportExcelService(
        [FromKeyedServices(AaveModuleKeyedService.DailyPlatformKeyService)]
        IPlatformDailyReportDataProvider platformDailyReportDataProvider)
    {
        _platformDailyReportDataProvider = platformDailyReportDataProvider;
    }

    public bool CanProcess(PlatformDailyReport dailyReport) => dailyReport is AaveDailyReport;

    public async Task CreateHeaderAsync(Spreadsheet workbook, Wallet wallet, CancellationToken ct = default)
    {
        await WriteWalletRow(workbook, wallet, ct);
        
        await workbook.AddHeaderRowAsync(AaveExcelReportContext.Default.AavePositionExcelRow, token: ct);
    }

    public async Task<Stream> CreateReportAsync(IReadOnlyCollection<Wallet> wallets, DateOnly? from, DateOnly? to,
        CancellationToken ct = default)
    {
        var (fromDate, toDate) = GetDefaultDatesIfNull(from, to);

        var positionReports =
            await _platformDailyReportDataProvider.GetReportDataAsync(wallets, fromDate, toDate, ct);

        var rowContext = AaveExcelReportContext.Default.AavePositionExcelRow;
        var ms = await CreateExcelWorkbookAsync(positionReports.PlatformName, rowContext, async workbook =>
        {
            foreach (var (wallet, positionReport) in positionReports.Reports)
            {
                await WriteWalletRow(workbook, wallet, ct);
                
                foreach (var platformDailyReport in positionReport)
                {
                    await WriteDataToWorksheetAsync(workbook, platformDailyReport, ct);
                }
            }
        }, ct);

        return ms;
    }

    public async Task WriteDataToWorksheetAsync(Spreadsheet workbook,
        PlatformDailyReport dailyReport,
        CancellationToken ct = default)
    {
        var aaveDailyReport = dailyReport as AaveDailyReport ??
                              throw new InvalidOperationException(
                                  "Platform daily report must be of type AaveDailyReport");

        foreach (var dailyReportItem in aaveDailyReport.ReportItems)
        {
            var row = dailyReportItem.MapToExcelRow();

            await workbook.AddAsRowAsync(row, AaveExcelReportContext.Default.AavePositionExcelRow, ct);
        }
        
        var totalRow = aaveDailyReport.MapToExcelModel(TotalName);

        await workbook.AddAsRowAsync(totalRow, AaveExcelReportContext.Default.AavePositionExcelTotalRow,
            ct);
        
        await workbook.AddRowAsync([], ct);
    }
}

[WorksheetRow(typeof(AavePositionExcelRow))]
[WorksheetRow(typeof(AavePositionExcelTotalRow))]
internal partial class AaveExcelReportContext : WorksheetRowContext;