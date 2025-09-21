using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Infrastructure.Excel;
using CryptoWatcher.Infrastructure.Uniswap.ExcelModels;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.UniswapModule.Extensions;
using Microsoft.Extensions.DependencyInjection;
using SpreadCheetah.SourceGeneration;

namespace CryptoWatcher.Infrastructure.Uniswap.Excel;

public interface IUniswapExcelReportService
{
    Task<Stream> CreateReportAsync(IReadOnlyCollection<Wallet> wallets, DateOnly? from, DateOnly? to,
        CancellationToken ct = default);
}

internal class UniswapExcelReportService : BaseExcelReportService, IUniswapExcelReportService
{
    private readonly IPlatformDailyReportDataProvider _dailyReportDataProvider;
    private readonly UniswapDailyReportExcelWorksheetWriter _worksheetWriter;

    public UniswapExcelReportService(
        [FromKeyedServices(UniswapModuleKeyedService.DailyPlatformKeyService)]
        IPlatformDailyReportDataProvider dailyReportDataProvider,
        UniswapDailyReportExcelWorksheetWriter worksheetWriter)
    {
        _dailyReportDataProvider = dailyReportDataProvider;
        _worksheetWriter = worksheetWriter;
    }

    public async Task<Stream> CreateReportAsync(
        IReadOnlyCollection<Wallet> wallets,
        DateOnly? from, DateOnly? to, CancellationToken ct = default)
    {
        var (fromDate, toDate) = GetDefaultDatesIfNull(from, to);

        var dailyReportData = await _dailyReportDataProvider.GetReportDataAsync(wallets, fromDate, toDate, ct);

        var rowContext = UniswapExcelRowContext.Default.UniswapPoolPositionExcelRow;

        return await CreateExcelWorkbookAsync(dailyReportData.PlatformName, _worksheetWriter, rowContext,
            dailyReportData, ct);
    }
}

[WorksheetRow(typeof(UniswapPoolPositionExcelRow))]
[WorksheetRow(typeof(UniswapPoolPositionExcelTotalRow))]
internal partial class UniswapExcelRowContext : WorksheetRowContext;