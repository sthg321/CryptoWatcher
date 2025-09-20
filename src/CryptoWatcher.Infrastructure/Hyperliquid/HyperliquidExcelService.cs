using CryptoWatcher.Abstractions;
using CryptoWatcher.HyperliquidModule.Extensions;
using CryptoWatcher.HyperliquidModule.Models;
using CryptoWatcher.Infrastructure.Aave;
using CryptoWatcher.Infrastructure.Excel;
using CryptoWatcher.Infrastructure.Hyperliquid.ExcelModels;
using CryptoWatcher.Infrastructure.Hyperliquid.Mappers;
using CryptoWatcher.Models;
using CryptoWatcher.Shared.Entities;
using Microsoft.Extensions.DependencyInjection;
using SpreadCheetah;
using SpreadCheetah.SourceGeneration;

namespace CryptoWatcher.Infrastructure.Hyperliquid;

/// <summary>
/// Defines methods for generating Excel reports based on Hyperliquid data.
/// </summary>
public interface IHyperliquidExcelService
{
    /// <summary>
    /// Generates a report based on the specified date range and returns it as a stream.
    /// </summary>
    /// <param name="wallets"></param>
    /// <param name="from">The starting date for the report. If null, the default start date is the first day of the current month.</param>
    /// <param name="to">The ending date for the report. If null, the default end date is the last day of the current month.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A stream containing the generated report in Excel format.</returns>
    Task<Stream> CreateReportAsync(IReadOnlyCollection<Wallet> wallets, DateOnly? from, DateOnly? to,
        CancellationToken ct = default);
}

internal class HyperliquidExcelService : BaseExcelReportService, IHyperliquidExcelService, IExcelSheetBuilder
{
    private const string EmptyValue = "-";

    private readonly IPlatformDailyReportDataProvider _platformDailyReportDataProvider;

    public HyperliquidExcelService(
        [FromKeyedServices(HyperliquidModuleKeyedService.DailyPlatformKeyService)]
        IPlatformDailyReportDataProvider platformDailyReportDataProvider)
    {
        _platformDailyReportDataProvider = platformDailyReportDataProvider;
    }

    public bool CanProcess(PlatformDailyReport dailyReport) => dailyReport is HyperliquidVaultReport;

    public async Task CreateHeaderAsync(Spreadsheet workbook, Wallet wallet, CancellationToken ct = default)
    {
        await workbook.AddRowAsync([new DataCell("Кошелек:"), new DataCell(wallet.Address)], ct);

        await workbook.AddHeaderRowAsync(
            HyperliquidVaultPositionExcelContext.Default.HyperliquidVaultPositionExcelRow,
            token: ct);
    }

    public async Task<Stream> CreateReportAsync(IReadOnlyCollection<Wallet> wallets, DateOnly? from, DateOnly? to,
        CancellationToken ct = default)
    {
        var (fromDate, toDate) = GetDefaultDatesIfNull(from, to);

        var platformDailyReports =
            await _platformDailyReportDataProvider.GetReportDataAsync(wallets, fromDate, toDate, ct);

        var rowContext = HyperliquidVaultPositionExcelContext.Default.HyperliquidVaultPositionExcelRow;

        var ms = await CreateExcelWorkbookAsync(platformDailyReports.PlatformName, rowContext, async workbook =>
        {
            foreach (var (wallet, dailyReports) in platformDailyReports.Reports)
            {
                await WriteWalletRow(workbook, wallet, ct);
                
                foreach (var dailyReport in dailyReports)
                {
                    await WriteDataToWorksheetAsync(workbook, dailyReport, ct);
                }
            }
        }, ct);

        return ms;
    }

    public async Task WriteDataToWorksheetAsync(Spreadsheet workbook,
        PlatformDailyReport dailyReport,
        CancellationToken ct = default)
    {
        var hyperliquidDailyReport = dailyReport as HyperliquidVaultReport ??
                                     throw new InvalidOperationException(
                                         "Platform daily report must be of type HyperliquidDailyReport");

        var rowContext = HyperliquidVaultPositionExcelContext.Default.HyperliquidVaultPositionExcelRow;
        var totalContext = HyperliquidVaultPositionExcelContext.Default.HyperliquidVaultPositionExcelTotalRow;

        foreach (var vaultReportItem in hyperliquidDailyReport.ReportItems)
        {
            await workbook.AddAsRowAsync(vaultReportItem.MapToExcelModel(), rowContext, ct);
        }

        await workbook.AddAsRowAsync(hyperliquidDailyReport.MapToExcelModel(TotalName, EmptyValue), totalContext, ct);
        
        await workbook.AddRowAsync([], ct);
    }
}

[WorksheetRow(typeof(HyperliquidVaultPositionExcelRow))]
[WorksheetRow(typeof(HyperliquidVaultPositionExcelTotalRow))]
internal partial class HyperliquidVaultPositionExcelContext : WorksheetRowContext;