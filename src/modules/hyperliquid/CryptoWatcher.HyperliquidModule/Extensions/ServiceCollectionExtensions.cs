using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.HyperliquidModule.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoWatcher.HyperliquidModule.Extensions;

public static class HyperliquidModuleKeyedService
{
    public const string DailyPlatformKeyService = nameof(HyperliquidReportDataService);
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHyperliquidModule(this IServiceCollection services)
    {
        services.AddScoped<IHyperliquidPositionsSyncService, HyperliquidPositionsSyncService>();
        services.AddKeyedSingleton<IPlatformDailyReportDataProvider, HyperliquidReportDataService>(HyperliquidModuleKeyedService
            .DailyPlatformKeyService);
 
        return services;
    }
}