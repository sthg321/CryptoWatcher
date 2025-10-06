using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Modules.Uniswap.Application.Services.Unichain;
using CryptoWatcher.UniswapModule.Abstractions;
using CryptoWatcher.UniswapModule.Services;
using CryptoWatcher.UniswapModule.Services.Unichain;
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
        services.AddKeyedScoped<IPlatformDailyReportDataProvider, UniswapReportService>(UniswapModuleKeyedService
            .DailyPlatformKeyService);
        services.AddSingleton<IUniswapMath, UniswapMath>();
        services.AddScoped<IUniswapPositionsSyncService, UniswapPositionsSyncService>();

        services.AddSingleton<IUnichainInternalTransactionProvider, UnichainInternalTransactionProvider>();
        services.AddSingleton<IUnichainLogProvider, UnichainLogProvider>();
        services.AddSingleton<IUnichainLogReader, UnichainLogReader>();
        services.AddSingleton<ILiquidityPoolEventDecoder, LiquidityPoolEventDecoder>();
        services.AddScoped<ILastProcessedBlockNumberProvider, LastProcessedBlockNumberProvider>();
        services.AddScoped<IUnichainEventFetcher, UnichainEventFetcher>();
        services.AddScoped<UnichainEventEnricher>();

        return services;
    }
}