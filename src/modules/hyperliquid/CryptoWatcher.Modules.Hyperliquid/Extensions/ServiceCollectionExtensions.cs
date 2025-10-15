using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Modules.Hyperliquid.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoWatcher.Modules.Hyperliquid.Extensions;

public static class HyperliquidModuleKeyedService
{
    public const string DailyPlatformKeyService = nameof(HyperliquidReportDataService);
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHyperliquidModule(this IServiceCollection services)
    {
        services.AddScoped<IHyperliquidPositionsSyncService, HyperliquidPositionsSyncService>();
        services.AddKeyedScoped<IPlatformDailyReportDataProvider, HyperliquidReportDataService>(HyperliquidModuleKeyedService
            .DailyPlatformKeyService);
 
        return services;
    }
}