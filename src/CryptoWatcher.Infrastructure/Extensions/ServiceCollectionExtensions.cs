using AaveClient.Extensions;
using CoinGeckoClient.Extensions;
using CryptoWatcher.AaveModule.Abstractions;
using CryptoWatcher.AaveModule.Extensions;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Application;
using CryptoWatcher.Application.Reports;
using CryptoWatcher.HyperliquidModule.Abstractions;
using CryptoWatcher.HyperliquidModule.Extensions;
using CryptoWatcher.Infrastructure.Aave;
using CryptoWatcher.Infrastructure.Aave.Excel;
using CryptoWatcher.Infrastructure.Configs;
using CryptoWatcher.Infrastructure.Hyperliquid;
using CryptoWatcher.Infrastructure.Hyperliquid.Excel;
using CryptoWatcher.Infrastructure.Integrations;
using CryptoWatcher.Infrastructure.Reports;
using CryptoWatcher.Infrastructure.Services;
using CryptoWatcher.Infrastructure.Uniswap;
using CryptoWatcher.Infrastructure.Uniswap.Excel;
using CryptoWatcher.Integrations;
using CryptoWatcher.UniswapModule.Abstractions;
using CryptoWatcher.UniswapModule.Extensions;
using CryptoWatcher.UniswapModule.Services;
using HyperliquidClient.Extensions;
using Microsoft.Extensions.DependencyInjection;
using UniswapClient.Extensions;

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

        services.AddScoped<ITokenEnricher, TokenEnricher>();

        services.AddSingleton<TokenService>();
        services.AddSingleton<TokenEnricher>();
        services.AddSingleton<CoinNormalizer>();

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

        services.AddScoped<IPoolHistorySyncRepositoryFacade, PoolHistorySyncRepositoryFacade>();

        services.AddCoinGeckoClient(provider => provider.GetRequiredService<ExternalServicesConfig>().CoinGecko);
        services.AddTransient<ICoinPriceProvider, CoinGeckoCoinPriceProvider>();

        services.AddSingleton<IAaveProvider, AaveProvider>();

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
            .AddScoped<IAaveReportExcelService, AaveReportExcelService>()
            .AddSingleton<IExcelSheetBuilder, AaveExcelSheetBuilder>()
            .AddSingleton<AaveDailyReportExcelWorksheetWriter>();

        return services;
    }

    private static IServiceCollection AddConfiguredHyperliquidModule(this IServiceCollection services)
    {
        services.AddHyperLiquidClient();

        services.AddHyperliquidModule()
            .AddScoped<IHyperliquidExcelService, HyperliquidExcelService>()
            .AddScoped<IHyperliquidProvider, HyperliquidApiProvider>()
            .AddSingleton<IExcelSheetBuilder, HyperliquidExcelSheetBuilder>()
            .AddSingleton<HyperliquidDailyReportExcelWorksheetWriter>();

        return services;
    }

    private static IServiceCollection AddConfiguredUniswapModule(this IServiceCollection services)
    {
        services.AddUniswapClient();

        services.AddUniswapModule()
            .AddSingleton<IUniswapProvider, UniswapProvider>()
            .AddScoped<IUniswapExcelReportService, UniswapExcelReportService>()
            .AddSingleton<IExcelSheetBuilder, UniswapExcelSheetBuilder>()
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