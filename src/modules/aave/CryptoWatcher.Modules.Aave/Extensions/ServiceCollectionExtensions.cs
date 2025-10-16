using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Modules.Aave.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoWatcher.Modules.Aave.Extensions;

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

        services.AddKeyedScoped<IPlatformDailyReportDataProvider, AaveReportDataService>(AaveModuleKeyedService
            .DailyPlatformKeyService);
        
        services.AddSingleton<IPlatformDailyReportDataProvider>(provider =>
            provider.GetRequiredKeyedService<IPlatformDailyReportDataProvider>(AaveModuleKeyedService
                .DailyPlatformKeyService));

        return services;
    }
}