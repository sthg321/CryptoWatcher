using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.UniswapModule.Services;
using CryptoWatcher.UniswapModule.Specifications;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoWatcher.UniswapModule.Extensions;

public static class UniswapModuleKeyedService
{
    public const string DailyPlatformKeyService = nameof(UniswapReportService);
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUniswapModule(this IServiceCollection services)
    {
        services.AddKeyedSingleton<IPlatformDailyReportDataProvider, UniswapReportService>(UniswapModuleKeyedService
            .DailyPlatformKeyService);
        services.AddSingleton<IUniswapMath, UniswapMath>();
        services.AddScoped<IUniswapPositionsSyncService, UniswapPositionsSyncService>();

        return services;
    }
}