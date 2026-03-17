using System.Threading.RateLimiting;
using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions.EventAppliers;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions.Reports;
using CryptoWatcher.Modules.Uniswap.Application.Services.DailyPerformance;
using CryptoWatcher.Modules.Uniswap.Application.Services.Reports;
using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization;
using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization;
using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3;
using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models
    .PositionEvents;
using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.
    PositionEventAppliers;
using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsSnapshotSynchronization;
using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.TransactionSynchronization;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Blockchain;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Blockchain.Api;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Blockchain.UniswapV3;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Blockchain.UniswapV3.LiquidityPool;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Blockchain.UniswapV3.LiquidityPoolFactory;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Blockchain.UniswapV3.PositionsFetcher;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Blockchain.UniswapV4;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Blockchain.UniswapV4.LiquidityPool;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Blockchain.UniswapV4.PositionsFetcher;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Blockchain.UniswapV4.StateView;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Kafka;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Services.Synchronization.PositionsEventsSynchronization;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.
    Abstractions;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.
    LogEventDecoders;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.
    Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Nethereum.ABI.ABIDeserialisation;
using Nethereum.RPC.Eth.Transactions;
using Polly;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Extensions;

public static class UniswapModuleKeyedService
{
    public const string DailyPlatformKeyService = nameof(UniswapDailyReportService);
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

        services.AddHostedService<BlockchainTransactionTransactionsConsumer>();
        services.AddScoped<UniswapChainConfigurationService>();
        services.AddScoped<IWalletTransactionConsumer, WalletTransactionConsumer>();

        services.AddScoped<IDailyPositionPerformanceSynchronizer, UniswapDailyPositionPerformanceSynchronizer>();

        services.AddSingleton<IUniswapProvider, UniswapProvider>();

        services.AddHttpClient("Web3")
            .AddResilienceHandler("Web3Resilience", builder =>
            {
                // Retry ПЕРВЫМ — оборачивает всё остальное
                builder.AddRetry(new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(1),
                    BackoffType = DelayBackoffType.Exponential,
                    ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                        .HandleResult(r =>
                            r.StatusCode is System.Net.HttpStatusCode.BadRequest
                                or System.Net.HttpStatusCode.TooManyRequests)
                        .Handle<HttpRequestException>(ex =>
                            ex.StatusCode is System.Net.HttpStatusCode.BadRequest
                                or System.Net.HttpStatusCode.TooManyRequests)
                });

                // Rate limiter ВТОРЫМ
                builder.AddRateLimiter(new SlidingWindowRateLimiter(new SlidingWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 10,
                    QueueLimit = int.MaxValue,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    SegmentsPerWindow = 10,
                    Window = TimeSpan.FromSeconds(1)
                }));
            });
        
        services.AddSingleton<IWeb3Factory, Web3Factory>();

        services.AddScoped<IUniswapOverallReportService, UniswapOverallReportService>();

        services.AddSingleton<IBlockchainGateway, Web3BlockchainGateway>();

        services.AddScoped<IUniswapTransactionEnricher, UniswapTransactionEnricher>();
        services.AddScoped<IUniswapWalletEventApplier, UniswapWalletEventApplier>();

        services.AddScoped<IUniswapTransactionEventSource, UniswapTransactionEventSource>();

        services.AddSingleton<IBlockchainGateway, Web3BlockchainGateway>();
        services.AddSingleton<IWeb3BlockchainApi, Web3BlockchainApi>();
        services.AddScoped<IUniswapTransactionEventSource, UniswapTransactionEventSource>();
        services.AddScoped<IUniswapPositionFromTransactionUpdater, UniswapPositionFromTransactionUpdater>();

        services.AddScoped<IPositionPriceSynchronizationJob, PositionSnapshotSynchronizationJob>();
        services.AddScoped<IPositionEvaluator, PositionEvaluator>();
        services.AddScoped<IPositionSnapshotUpdater, PositionSnapshotUpdater>();

        services.AddScoped<IUniswapPositionUpdater, UniswapPositionUpdater>();
        services.AddScoped<IUniswapPositionTransactionSynchronizer, UniswapPositionTransactionSynchronizer>();

        //v3
        services.AddSingleton<UniswapV3Client>();
        services.AddSingleton<IUniswapV3LiquidityPool, UniswapV3LiquidityPool>();
        services.AddSingleton<IUniswapV3PoolFactory, UniswapV3PoolFactory>();
        services.AddSingleton<IUniswapV3PositionFetcher, UniswapV3PositionFetcher>();

        services.AddSingleton<IPositionEventSource, UniswapV3PositionEventSource>();
        services.AddSingleton<IUniswapTransactionFilter, UniswapV3TransactionFilter>();
        services.AddSingleton<IUniswapTransactionLogsDecoderFactory, UniswapV3TransactionLogsDecoderFactory>();
        services.AddSingleton<IUniswapPositionEventApplier, UniswapV3PositionEventApplier>();

        services.AddSingleton<IPositionEventApplierFactory, PositionEventApplierFactory>();
        services.AddSingleton<IUniswapLiquidityPositionEventReducer, UniswapLiquidityPositionEventReducer>();
        services.AddSingleton<IPositionMintEventApplier, PositionMintEventApplier>();
        services.AddSingleton<ITransactionLogEventDecoder, UniswapV3CollectLogEventDecoder>();
        services.AddSingleton<ITransactionLogEventDecoder, UniswapV3DecreaseLiquidityLogEventDecoder>();
        services.AddSingleton<ITransactionLogEventDecoder, UniswapV3IncreaseLiquidityLogEventDecoder>();
        services.AddSingleton<ITransactionLogEventDecoder, UniswapV3MintLogEventDecoder>();

        services.AddUniswapOperationAppliers();

        //v4
        services.AddSingleton<UniswapV4Client>();
        services.AddSingleton<IUniswapV4StateView, UniswapV4StateView>();
        services.AddSingleton<IUniswapV4LiquidityPool, UniswapV4LiquidityPool>();
        services.AddSingleton<IUniswapV4PositionFetcher, UniswapV4PositionFetcher>();

        services.AddKeyedScoped<IPlatformDailyReportDataProvider, UniswapDailyReportService>(UniswapModuleKeyedService
            .DailyPlatformKeyService);
        services.AddSingleton<IUniswapMath, UniswapMath>();

        return services;
    }

    private static IServiceCollection AddUniswapOperationAppliers(this IServiceCollection services)
    {
        // Keyed generics с factory func
        RegisterKeyedApplier<CollectFeesEvent, CollectFeesEventApplier>(services);
        RegisterKeyedApplier<IncreaseLiquidityEvent, IncreaseLiquidityEventApplier>(services);
        RegisterKeyedApplier<DecreaseLiquidityEvent, DecreaseLiquidityEventApplier>(services);

        // Фабрика
        services.AddScoped<PositionEventApplierFactory>();

        return services;
    }

    private static void RegisterKeyedApplier<TOperation, TApplier>(IServiceCollection services)
        where TOperation : PositionEvent
        where TApplier : class, IPositionMutationEvent
    {
        services.AddSingleton<TApplier>();

        var key = typeof(TOperation);
        services.AddKeyedSingleton<IPositionMutationEvent>(key, (sp, _) => sp.GetRequiredService<TApplier>());
    }
}