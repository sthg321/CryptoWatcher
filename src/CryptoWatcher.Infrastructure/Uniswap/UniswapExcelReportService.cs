using CryptoWatcher.Abstractions;
using CryptoWatcher.Infrastructure.Aave;
using CryptoWatcher.Infrastructure.Excel;
using CryptoWatcher.Infrastructure.Uniswap.ExcelModels;
using CryptoWatcher.Infrastructure.Uniswap.Mappers;
using CryptoWatcher.Models;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.UniswapModule.Extensions;
using CryptoWatcher.UniswapModule.Models;
using Microsoft.Extensions.DependencyInjection;
using SpreadCheetah;
using SpreadCheetah.SourceGeneration;

namespace CryptoWatcher.Infrastructure.Uniswap;

public interface IUniswapExcelReportService
{
    Task<Stream> CreateReportAsync(IReadOnlyCollection<Wallet> wallets, DateOnly? from, DateOnly? to,
        CancellationToken ct = default);
}

internal class UniswapExcelReportService : BaseExcelReportService, IUniswapExcelReportService, IExcelSheetBuilder
{
    private readonly IPlatformDailyReportDataProvider _dailyReportDataProvider;

    public UniswapExcelReportService(
        [FromKeyedServices(UniswapModuleKeyedService.DailyPlatformKeyService)]
        IPlatformDailyReportDataProvider dailyReportDataProvider)
    {
        _dailyReportDataProvider = dailyReportDataProvider;
    }

    public bool CanProcess(PlatformDailyReport dailyReport) => dailyReport is UniswapDailyReport;

    public async Task CreateHeaderAsync(Spreadsheet workbook, Wallet wallet, CancellationToken ct = default)
    {
        var rowContext = PoolInfoExcelRowContext.Default.UniswapPoolPositionExcelRow;

        await WriteWalletRow(workbook, wallet, ct);

        await workbook.AddHeaderRowAsync(rowContext, token: ct);
    }

    public async Task<Stream> CreateReportAsync(
        IReadOnlyCollection<Wallet> wallets,
        DateOnly? from, DateOnly? to, CancellationToken ct = default)
    {
        var (fromDate, toDate) = GetDefaultDatesIfNull(from, to);

        var dailyReportData = await _dailyReportDataProvider.GetReportDataAsync(wallets, fromDate, toDate, ct);

        var rowContext = PoolInfoExcelRowContext.Default.UniswapPoolPositionExcelRow;

        var ms = await CreateExcelWorkbookAsync(dailyReportData.PlatformName, rowContext, async workbook =>
        {
            foreach (var (wallet, poolPositions) in dailyReportData.Reports)
            {
                await WriteWalletRow(workbook, wallet, ct);
                
                foreach (var poolPosition in poolPositions)
                {
                    await WriteDataToWorksheetAsync(workbook, poolPosition, ct);
                }
            }
        }, ct);

        return ms;
    }

    public async Task WriteDataToWorksheetAsync(Spreadsheet workbook, PlatformDailyReport dailyReport,
        CancellationToken ct = default)
    {
        var hyperliquidDailyReport = dailyReport as UniswapDailyReport ??
                                     throw new InvalidOperationException(
                                         "Platform daily report must be of type AaveDailyReport");

        var rowContext = PoolInfoExcelRowContext.Default.UniswapPoolPositionExcelRow;
        var totalContext = PoolInfoExcelRowContext.Default.UniswapPoolPositionExcelTotalRow;

        foreach (var dailyReportItem in hyperliquidDailyReport.ReportItems)
        {
            var row = dailyReportItem.MapToExcelRowModel();

            await workbook.AddAsRowAsync(row, rowContext, ct);
        }

        var lastItem = hyperliquidDailyReport.ReportItems.Last();

        var totalRow = hyperliquidDailyReport.MapToExcelModel(TotalName, lastItem.TokenPairSymbols, lastItem.Network);

        await workbook.AddAsRowAsync(totalRow, totalContext, ct);

        await workbook.AddRowAsync([], ct);
    }
}

[WorksheetRow(typeof(UniswapPoolPositionExcelRow))]
[WorksheetRow(typeof(UniswapPoolPositionExcelTotalRow))]
internal partial class PoolInfoExcelRowContext : WorksheetRowContext;