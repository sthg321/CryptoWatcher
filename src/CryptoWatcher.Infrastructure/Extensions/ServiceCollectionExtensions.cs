using AaveClient.Extensions;
using CoinGeckoClient.Extensions;
using CryptoWatcher.AaveModule.Abstractions;
using CryptoWatcher.AaveModule.Extensions;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Application;
using CryptoWatcher.Application.Reports;
using CryptoWatcher.Modules.Hyperliquid.Abstractions;
using CryptoWatcher.Infrastructure.Aave;
using CryptoWatcher.Infrastructure.Configs;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Aave;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Abstractions;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Hyperliquid;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Total;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Uniswap;
using CryptoWatcher.Infrastructure.Integrations;
using CryptoWatcher.Infrastructure.Services;
using CryptoWatcher.Integrations;
using CryptoWatcher.Modules.Hyperliquid.Infrastructure.Client.Extensions;
using CryptoWatcher.Modules.Hyperliquid.Infrastructure.Extensions;
using CryptoWatcher.Modules.Hyperliquid.Infrastructure.Services;
using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Services;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoWatcher.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services
            .AddConfiguredAaveModule()
            .AddConfiguredHyperliquidModule()
            .AddConfiguredUniswapModule()
            .AddConfiguredApplication();

        services.AddSingleton<ITokenEnricher, TokenEnricher>();

        services.AddSingleton<TokenService>();
        services.AddSingleton<TokenEnricher>();
        services.AddSingleton<CoinNormalizer>();
        services.AddSingleton<IDailyTotalReportWorksheetBuilder, DailyTotalReportWorksheetBuilder>();

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

        services.AddScoped<IPoolHistorySyncRepositoryFacade, PoolHistorySyncRepositoryFacade>();

        services.AddCoinGeckoClient(provider => provider.GetRequiredService<ExternalServicesConfig>().CoinGecko);
        services.AddTransient<ICoinPriceProvider, CoinGeckoCoinPriceProvider>();

        services.AddSingleton<IAaveProvider, AaveProvider>();

        services.AddSingleton<IExcelReportGenerator, ExcelReportGenerator>();
        services.AddScoped<IPlatformDailyReportFacade, PlatformDailyReportFacade>();
        return services;
    }

    private static IServiceCollection AddConfiguredApplication(this IServiceCollection services)
    {
        services 
            .AddScoped<IEnumerable<IPlatformDailyReportDataProvider>>(GetPlatformReportProviders)
            .AddScoped<IDailySummaryReportProvider, DailySummaryReportProvider>()
            .AddSingleton<IDailySummaryReportBuilder, DailySummaryReportBuilder>();

        return services;
    }

    private static IServiceCollection AddConfiguredAaveModule(this IServiceCollection services)
    {
        services.AddAaveClient();

        services
            .AddAaveModule()
            .AddSingleton<IAaveMainnetProvider, AaveMainnetProvider>()
            .AddScoped<IAaveProvider, AaveProvider>()
            .AddSingleton<IDailyExcelSheetBuilder, AaveDailyExcelSheetBuilder>()
            .AddSingleton<AaveDailyReportExcelWorksheetWriter>();

        return services;
    }

    private static IServiceCollection AddConfiguredHyperliquidModule(this IServiceCollection services)
    {
        services.AddHyperLiquidClient();

        services.AddHyperliquidModule()
            .AddSingleton<IDailyExcelSheetBuilder, HyperliquidDailyExcelSheetBuilder>()
            .AddSingleton<HyperliquidDailyReportExcelWorksheetWriter>();

        return services;
    }

    private static IServiceCollection AddConfiguredUniswapModule(this IServiceCollection services)
    { 
        services.AddUniswapModule()
            .AddSingleton<IDailyExcelSheetBuilder, UniswapDailyExcelSheetBuilder>()
            .AddSingleton<UniswapDailyReportExcelWorksheetWriter>();

        return services;
    }

    private static List<IPlatformDailyReportDataProvider> GetPlatformReportProviders(IServiceProvider provider)
    {
        return
        [
            provider.GetRequiredKeyedService<IPlatformDailyReportDataProvider>(
                AaveModuleKeyedService.DailyPlatformKeyService),
            provider.GetRequiredKeyedService<IPlatformDailyReportDataProvider>(
                HyperliquidModuleKeyedService.DailyPlatformKeyService),
            provider.GetRequiredKeyedService<IPlatformDailyReportDataProvider>(
                UniswapModuleKeyedService.DailyPlatformKeyService)
        ];
    }
}