using CryptoWatcher.HyperliquidModule.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoWatcher.HyperliquidModule.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHyperliquidModule(this IServiceCollection services)
    {
        services.AddScoped<IHyperliquidPositionsSyncService, HyperliquidPositionsSyncService>();
        services.AddScoped<IHyperliquidReportService, HyperliquidReportService>();
        return services;
    }
}