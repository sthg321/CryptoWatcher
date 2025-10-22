using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Modules.Aave.Abstractions;
using CryptoWatcher.Modules.Aave.Application.Abstractions;
using CryptoWatcher.Modules.Aave.Application.Abstractions.Client;
using CryptoWatcher.Modules.Aave.Application.Services;
using CryptoWatcher.Modules.Aave.Infrastructure.Client;
using CryptoWatcher.Modules.Aave.Infrastructure.Client.UiPoolDataProvider;
using CryptoWatcher.Modules.Aave.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Nethereum.ABI.ABIDeserialisation;

namespace CryptoWatcher.Modules.Aave.Infrastructure.Extensions;

public static class AaveModuleKeyedService
{
    public const string DailyPlatformKeyService = nameof(AaveReportDataService);
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAaveModule(this IServiceCollection services)
    {
        AbiDeserializationSettings.UseSystemTextJson = true;
        
        services.AddSingleton<IUiPoolDataProviderFetcher, UiPoolDataProviderFetcher>();

        services.AddSingleton<IAaveApiClient, AaveApiClient>();
        
        services.AddScoped<IAavePositionsSyncService, AavePositionsSyncService>();
        services.AddScoped<IAaveTokenEnricher, AaveTokenEnricher>();

        services.AddScoped<IAaveProvider, AaveProvider>();

        services.AddKeyedScoped<IPlatformDailyReportDataProvider, AaveReportDataService>(AaveModuleKeyedService
            .DailyPlatformKeyService);

        services.AddSingleton<IPlatformDailyReportDataProvider>(provider =>
            provider.GetRequiredKeyedService<IPlatformDailyReportDataProvider>(AaveModuleKeyedService
                .DailyPlatformKeyService));

        return services;
    }
}