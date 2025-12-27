using CoinGeckoClient.Extensions;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Application;
using CryptoWatcher.Application.Abstractions;
using CryptoWatcher.Application.Reports;
using CryptoWatcher.Infrastructure.BackgroundServices;
using CryptoWatcher.Infrastructure.Configs;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Aave;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Abstractions;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Hyperliquid;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Total;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Uniswap;
using CryptoWatcher.Infrastructure.Integrations;
using CryptoWatcher.Infrastructure.Services;
using CryptoWatcher.Infrastructure.Telegram;
using CryptoWatcher.Integrations;
using CryptoWatcher.Modules.Aave.Infrastructure.Extensions;
using CryptoWatcher.Modules.Hyperliquid.Application.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Infrastructure.Client.Extensions;
using CryptoWatcher.Modules.Hyperliquid.Infrastructure.Extensions;
using CryptoWatcher.Modules.Morpho.Infrastructure.Extensions;
using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Services;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace CryptoWatcher.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTelegram(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TelegramConfig>(configuration.GetSection(nameof(TelegramConfig)));
        services.AddSingleton(provider => provider.GetRequiredService<IOptions<TelegramConfig>>().Value);
        services.AddSingleton<TelegramReportHandler>();
        
        services.AddHostedService<TelegramBackgroundService>();
      
        services.AddSingleton<TelegramBotClient>(provider =>
        {
            var client = new TelegramBotClient(provider.GetRequiredService<TelegramConfig>().BotToken);

            return client;
        });

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services
            .AddConfiguredAaveModule()
            .AddConfiguredHyperliquidModule()
            .AddConfiguredUniswapModule()
            .AddMorphoModule(provider => provider.GetRequiredService<ExternalServicesConfig>().Morpho)
            .AddConfiguredApplication();

        services.AddSingleton<ITokenEnricher, TokenEnricher>();

        services.AddSingleton<TokenService>();
        services.AddSingleton<TokenEnricher>();
        services.AddSingleton<CoinNormalizer>();
        services.AddSingleton<IDailyTotalReportWorksheetBuilder, DailyTotalReportWorksheetBuilder>();

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

        services.AddScoped<IPoolHistorySyncRepositoryFacade, PoolHistorySyncRepositoryFacade>();
        services.AddScoped<IHyperliquidSyncRepoFacade, HyperliquidSyncRepoFacade>();

        services.AddCoinGeckoClient(provider => provider.GetRequiredService<ExternalServicesConfig>().CoinGecko);
        services.AddTransient<ICoinPriceProvider, CoinGeckoCoinPriceProvider>();

        services.AddSingleton<IExcelReportGenerator, ExcelReportGenerator>();
        services.AddScoped<IPlatformDailyReportFacade, PlatformDailyReportFacade>();

        services.AddScoped<IDailyPositionPerformanceCoordinator, DailyPositionPerformanceCoordinator>();

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
        services.AddAaveModule()
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