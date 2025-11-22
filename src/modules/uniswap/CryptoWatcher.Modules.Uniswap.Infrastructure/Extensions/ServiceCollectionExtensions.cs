using System.Threading.RateLimiting;
using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Services;
using CryptoWatcher.Modules.Uniswap.Application.Services.EventsSynchronization;
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
using CryptoWatcher.Modules.Uniswap.Infrastructure.Services.EventsSynchronization;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Services.EventsSynchronization.V3;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Services.EventsSynchronization.V4;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Services.PositionsSynchronization;
using Microsoft.Extensions.DependencyInjection;
using Nethereum.ABI.ABIDeserialisation;
using Polly;
using Polly.RateLimiting;

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

        services.AddMemoryCache();

        services.AddResiliencePipeline("Uniswap", builder =>
        {
            builder.AddRateLimiter(new SlidingWindowRateLimiter(new SlidingWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 20,
                QueueLimit = int.MaxValue,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                SegmentsPerWindow = 10,
                Window = TimeSpan.FromSeconds(1)
            }));
        });

        services.AddScoped<IDailyPositionPerformanceSynchronizer, UniswapDailyPositionPerformanceSynchronizer>();
        
        services.AddScoped<IBlockscoutTransactionSynchronizer, BlockscoutTransactionSynchronizer>();
        services.AddSingleton<IBlockscoutTransactionFetcher, BlockscoutTransactionFetcher>();
        services.AddScoped<IUniswapChainSynchronizerOrchestrator, UniswapChainSynchronizationOrchestrator>();
        services.AddScoped<IUniswapChainSynchronizer, UniswapChainSynchronizer>();
        services.AddScoped<IUniswapCashFlowBlockRangeSynchronizer, UniswapCashFlowBlockRangeSynchronizer>();

        services.AddSingleton<IUniswapProvider, UniswapProvider>();
        services.AddSingleton<IChainLogChunkingStrategy, ChainLogChunkingStrategy>();
        services.AddSingleton<ICashFlowEventMatcher, CashFlowEventMatcher>();
        services.AddSingleton<ILiquidityEventsProvider, LiquidityEventsProvider>();
        services.AddSingleton<ITransactionDataProvider, Web3TransactionDataProvider>();
        services.AddHttpClient<IBlockscoutProvider, BlockscoutProvider>()
            .AddStandardResilienceHandler();

        services.AddSingleton<IUniswapLiquidityPoolEventDecoderSelector, UniswapLiquidityPoolEventDecoderSelector>();
        
        services.AddSingleton<ILiquidityEventLogEnricher, LiquidityEventLogEnricher>();
        services.AddSingleton<IWeb3Factory, Web3Factory>();

        //v3
        services.AddSingleton<UniswapV3Client>();
        services.AddSingleton<ILiquidityPoolEventDecoder, UniswapV3CollectEventDecoder>();
        services.AddSingleton<IBlockchainLogProvider, UniswapV3BlockchainLogProvider>();
        services.AddSingleton<IUniswapV3LiquidityPool, UniswapV3LiquidityPool>();
        services.AddSingleton<IUniswapV3PoolFactory, UniswapV3PoolFactory>();
        services.AddSingleton<IUniswapV3PositionFetcher, UniswapV3PositionFetcher>();

        //v4
        services.AddSingleton<UniswapV4Client>();
        services.AddSingleton<ILiquidityPoolEventDecoder, UniswapV4ModifyLiquidityEventDecoder>();
        services.AddSingleton<IBlockchainLogProvider, UniswapV4BlockchainLogProvider>();
        services.AddSingleton<IUniswapV4StateView, UniswapV4StateView>();
        services.AddSingleton<IUniswapV4LiquidityPool, UniswapV4LiquidityPool>();
        services.AddSingleton<IUniswapV4PositionFetcher, UniswapV4PositionFetcher>();
        services.AddHttpClient<UniswapAppApiClient>(client =>
            client.BaseAddress = new Uri("https://interface.gateway.uniswap.org")).AddStandardHedgingHandler();

        services.AddKeyedScoped<IPlatformDailyReportDataProvider, UniswapReportService>(UniswapModuleKeyedService
            .DailyPlatformKeyService);
        services.AddSingleton<IUniswapMath, UniswapMath>();
        services.AddScoped<IUniswapPositionsSyncService, UniswapPositionsSyncService>();


        return services;
    }
}