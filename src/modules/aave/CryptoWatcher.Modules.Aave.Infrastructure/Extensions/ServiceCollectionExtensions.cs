using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Application.Abstractions;
using CryptoWatcher.Modules.Aave.Application.Abstractions;
using CryptoWatcher.Modules.Aave.Application.Services;
using CryptoWatcher.Modules.Aave.Infrastructure.Integrations.Blockchain;
using CryptoWatcher.Modules.Aave.Infrastructure.Integrations.Blockchain.UiPoolDataProvider;
using CryptoWatcher.Modules.Aave.Infrastructure.Persistence;
using CryptoWatcher.Modules.Aave.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nethereum.ABI.ABIDeserialisation;

namespace CryptoWatcher.Modules.Aave.Infrastructure.Extensions;

public static class AaveModuleKeyedService
{
    public const string DailyPlatformKeyService = nameof(AaveReportDataService);
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAaveModule(this IServiceCollection services, string connectionString)
    {
        AbiDeserializationSettings.UseSystemTextJson = true;

        services.AddDbContext<AaveDbContext>(builder => builder.UseNpgsql(connectionString,
            npgsql => npgsql
                .MigrationsHistoryTable("__EFMigrationsHistory", "aave")
                .MigrationsAssembly(typeof(AaveDbContext).Assembly.FullName)));
        
        services.AddSingleton<IUiPoolDataProviderFetcher, UiPoolDataProviderFetcher>();

        services.AddSingleton<IAaveGateway, AaveGateway>();
        services.AddSingleton<IAaveHealthFactorCalculator, AaveHealthFactorCalculator>();

        services.AddScoped<IAavePositionRepository, AavePositionRepository>();
        services.AddScoped<IDailyPositionPerformanceSynchronizer, AaveDailyPositionPerformanceSynchronizer>();
        
        services.AddScoped<IAavePositionsSyncService, AavePositionsSyncService>();
        services.AddScoped<IAaveTokenEnricher, AaveTokenEnricher>();

        services.AddScoped<IAaveProvider, AaveProvider>();
        services.AddScoped<AaveAccountStatusService>();

        services.AddKeyedScoped<IPlatformDailyReportDataProvider, AaveReportDataService>(AaveModuleKeyedService
            .DailyPlatformKeyService);

        services.AddSingleton<IPlatformDailyReportDataProvider>(provider =>
            provider.GetRequiredKeyedService<IPlatformDailyReportDataProvider>(AaveModuleKeyedService
                .DailyPlatformKeyService));

        return services;
    }
}