using CryptoWatcher.AaveModule.Services;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.Reports;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoWatcher.AaveModule.Extensions;

public static class AaveModuleKeyedService
{
    public const string DailyPlatformKeyService = nameof(AaveReportDataService);
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAaveModule(this IServiceCollection services)
    {
        services.AddScoped<IAavePositionsSyncService, AavePositionsSyncService>();
        services.AddScoped<IAaveTokenEnricher, AaveTokenEnricher>();

        services.AddKeyedSingleton<IPlatformDailyReportDataProvider, AaveReportDataService>(AaveModuleKeyedService
            .DailyPlatformKeyService);
        
        services.AddSingleton<IPlatformDailyReportDataProvider>(provider =>
            provider.GetRequiredKeyedService<IPlatformDailyReportDataProvider>(AaveModuleKeyedService
                .DailyPlatformKeyService));

        return services;
    }
}