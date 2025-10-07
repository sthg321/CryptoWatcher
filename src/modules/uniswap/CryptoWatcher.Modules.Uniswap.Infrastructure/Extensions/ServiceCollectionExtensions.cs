using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Services.Unichain;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV3;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV3.LiquidityPool;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV3.LiquidityPoolFactory;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV3.PositionsFetcher;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV4;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV4.LiquidityPool;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV4.PositionsFetcher;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV4.StateView;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV4.UniswapAppApiClient;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Services;
using CryptoWatcher.UniswapModule.Services;
using CryptoWatcher.UniswapModule.Specifications;
using Microsoft.Extensions.DependencyInjection;
using Nethereum.ABI.ABIDeserialisation;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Extensions;

public static class UniswapModuleKeyedService
{
    public const string DailyPlatformKeyService = nameof(UniswapReportService);
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUniswapModule(this IServiceCollection services)
    {
        AbiDeserializationSettings.UseSystemTextJson = true;

        services.AddSingleton<IUniswapProvider, UniswapProvider>();
        //v3
        services.AddSingleton<UniswapV3Client>();
        services.AddSingleton<IUniswapV3LiquidityPool, UniswapV3LiquidityPool>();
        services.AddSingleton<IUniswapV3PoolFactory, UniswapV3PoolFactory>();
        services.AddSingleton<IUniswapV3PositionFetcher, UniswapV3PositionFetcher>();

        //v4
        services.AddSingleton<UniswapV4Client>();
        services.AddSingleton<IUniswapV4StateView, UniswapV4StateView>();
        services.AddSingleton<IUniswapV4LiquidityPool, UniswapV4LiquidityPool>();
        services.AddSingleton<IUniswapV4PositionFetcher, UniswapV4PositionFetcher>();
        services.AddSingleton<UniswapAppApiClient>(_ => new UniswapAppApiClient(new HttpClient
        {
            BaseAddress = new Uri("https://interface.gateway.uniswap.org")
        }));

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