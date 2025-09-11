using CryptoWatcher.AaveModule.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoWatcher.AaveModule.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAaveModule(this IServiceCollection services)
    {
        services.AddScoped<IAavePositionsSyncService, AavePositionsSyncService>();
        services.AddScoped<IAaveTokenEnricher, AaveTokenEnricher>();
        services.AddScoped<IAaveReportService, AaveReportService>();
        return services;
    }
}