using CryptoWatcher.AaveModule.Extensions;
using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Aave;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Hyperliquid;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Uniswap;
using CryptoWatcher.Models;
using CryptoWatcher.Modules.Hyperliquid.Infrastructure.Extensions;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Extensions;
using CryptoWatcher.Shared.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoWatcher.Infrastructure.Excel.PlatformDailyReports;

public interface IPlatformDailyReportFacade
{
    Task<ExcelReport> CreateHyperliquidReportAsync(IReadOnlyCollection<Wallet> wallets, DateOnly? from, DateOnly? to,
        CancellationToken ct = default);

    Task<ExcelReport> CreateUniswapReportAsync(IReadOnlyCollection<Wallet> wallets, DateOnly? from, DateOnly? to,
        CancellationToken ct = default);

    Task<ExcelReport> CreateAaveReportAsync(IReadOnlyCollection<Wallet> wallets, DateOnly? from, DateOnly? to,
        CancellationToken ct = default);
}

internal class PlatformDailyReportFacade : IPlatformDailyReportFacade
{
    private readonly IExcelReportGenerator _excelReportGenerator;
    private readonly IServiceProvider _serviceProvider;

    public PlatformDailyReportFacade(
        IExcelReportGenerator excelReportGenerator,
        IServiceProvider serviceProvider)
    {
        _excelReportGenerator = excelReportGenerator;
        _serviceProvider = serviceProvider;
    }   

    public async Task<ExcelReport> CreateHyperliquidReportAsync(IReadOnlyCollection<Wallet> wallets, DateOnly? from,
        DateOnly? to, CancellationToken ct = default)
    {
        var dataProvider = _serviceProvider.GetRequiredKeyedService<IPlatformDailyReportDataProvider>(
            HyperliquidModuleKeyedService.DailyPlatformKeyService);

        var worksheetWriter = _serviceProvider.GetRequiredService<HyperliquidDailyReportExcelWorksheetWriter>();

        return await _excelReportGenerator.CreatePlatformDailyReportAsync(
            dataProvider,
            worksheetWriter,
            wallets,
            from,
            to,
            "hyperliquid",
            ct);
    }

    public async Task<ExcelReport> CreateUniswapReportAsync(IReadOnlyCollection<Wallet> wallets, DateOnly? from, DateOnly? to,
        CancellationToken ct = default)
    {
        var dataProvider = _serviceProvider.GetRequiredKeyedService<IPlatformDailyReportDataProvider>(
            UniswapModuleKeyedService.DailyPlatformKeyService);

        var worksheetWriter = _serviceProvider.GetRequiredService<UniswapDailyReportExcelWorksheetWriter>();

        return await _excelReportGenerator.CreatePlatformDailyReportAsync(
            dataProvider,
            worksheetWriter,
            wallets,
            from,
            to,
            "uniswap",
            ct);
    }

    public async Task<ExcelReport> CreateAaveReportAsync(IReadOnlyCollection<Wallet> wallets, DateOnly? from,
        DateOnly? to, CancellationToken ct = default)
    {
        var dataProvider = _serviceProvider.GetRequiredKeyedService<IPlatformDailyReportDataProvider>(
            AaveModuleKeyedService.DailyPlatformKeyService);

        var worksheetWriter = _serviceProvider.GetRequiredService<AaveDailyReportExcelWorksheetWriter>();

        return await _excelReportGenerator.CreatePlatformDailyReportAsync(
            dataProvider,
            worksheetWriter,
            wallets,
            from,
            to,
            "aave",
            ct);
    }
}