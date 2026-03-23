using System.Threading.RateLimiting;
using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions.EventAppliers;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions.Reports;
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
using CryptoWatcher.Modules.Uniswap.Entities;
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
using CryptoWatcher.Modules.Uniswap.Infrastructure.Persistence;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Persistence.Repositories;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Services.Synchronization.PositionsEventsSynchronization;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.
    Abstractions;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.
    LogEventDecoders;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.
    Services;
using CryptoWatcher.Modules.Uniswap.ValueObjects;
using CryptoWatcher.ValueObjects;
using Microsoft.EntityFrameworkCore;
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
    public static IServiceCollection AddUniswapModule(this IServiceCollection services, string connectionString)
    {
        AbiDeserializationSettings.UseSystemTextJson = true;

        services.AddMemoryCache();
        services.AddDbContext<UniswapDbContext>(options =>
            
            options
                .UseSeeding((context, _) => SeedUniswapChainData(context))
                .UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "uniswap");
                npgsql.MigrationsAssembly(typeof(UniswapDbContext).Assembly.FullName);
            }));

        services.AddScoped<IUniswapLiquidityPositionRepository, UniswapLiquidityPositionRepository>();
        services.AddScoped<IUniswapChainConfigurationRepository, UniswapChainConfigurationRepository>();
        
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
    
    
    private static void SeedUniswapChainData(DbContext context)
    {
        if (!context.Set<UniswapChainConfiguration>().Any())
        {
            context.Set<UniswapChainConfiguration>().Add(new UniswapChainConfiguration
            {
                Name = "Unichain",
                ChainId = 130,
                RpcUrl = new Uri("https://lb.drpc.live/unichain"),
                BlockscoutUrl = new Uri("https://unichain.blockscout.com"),
                SmartContractAddresses = new UniswapAddresses
                {
                    PoolFactory = EvmAddress.Create("0x1f98400000000000000000000000000000000004"),
                    MultiCall = EvmAddress.Create("0xb7610f9b733e7d45184be3a1bc966960ccc54f0b"),
                    PositionManager =
                        EvmAddress.Create("0x4529A01c7A0410167c5740C487A8DE60232617bf"),
                    StateView = EvmAddress.Create("0x86e8631A016F9068C3f085fAF484Ee3F5fDee8f2")
                },
                ProtocolVersion = UniswapProtocolVersion.V4
            });
            
            context.Set<UniswapChainConfiguration>().Add(new UniswapChainConfiguration
            {
                Name = "Arbitrum",
                ChainId = 42161,
                RpcUrl = new Uri("https://lb.drpc.live/arbitrum"),
                BlockscoutUrl = new Uri("https://arbitrum.blockscout.com"),
                SmartContractAddresses = new UniswapAddresses
                {
                    PoolFactory = EvmAddress.Create("0x360e68faccca8ca495c1b759fd9eee466db9fb32"),
                    MultiCall = EvmAddress.Create("0x842eC2c7D803033Edf55E478F461FC547Bc54EB2"),
                    PositionManager = EvmAddress.Create("0xd88F38F930b7952f2DB2432Cb002E7abbF3dD869"),
                    StateView = EvmAddress.Create("0x76Fd297e2D437cd7f76d50F01AfE6160f86e9990")
                },
                ProtocolVersion = UniswapProtocolVersion.V4
            });
            
            context.Set<UniswapChainConfiguration>().Add(new UniswapChainConfiguration
            {
                Name = "Arbitrum",
                ChainId = 42161,
                RpcUrl = new Uri("https://lb.drpc.live/arbitrum"),
                BlockscoutUrl = new Uri("https://arbitrum.blockscout.com"),
                SmartContractAddresses = new UniswapAddresses
                {
                    PoolFactory = EvmAddress.Create("0x1F98431c8aD98523631AE4a59f267346ea31F984"),
                    MultiCall = EvmAddress.Create("0xcA11bde05977b3631167028862bE2a173976CA11"),
                    PositionManager = EvmAddress.Create("0xC36442b4a4522E871399CD717aBDD847Ab11FE88")
                },
                ProtocolVersion = UniswapProtocolVersion.V3
            });
            
            context.Set<UniswapChainConfiguration>().Add(new UniswapChainConfiguration
            {
                Name = "Ethereum",
                ChainId = 1,
                RpcUrl = new Uri("https://lb.drpc.live/ethereum"),
                BlockscoutUrl = new Uri("https://etherscan.io"),
                SmartContractAddresses = new UniswapAddresses
                {
                    PoolFactory = EvmAddress.Create("0x1F98431c8aD98523631AE4a59f267346ea31F984"),
                    MultiCall = EvmAddress.Create("0xcA11bde05977b3631167028862bE2a173976CA11"),
                    PositionManager = EvmAddress.Create("0xC36442b4a4522E871399CD717aBDD847Ab11FE88")
                },
                ProtocolVersion = UniswapProtocolVersion.V3
            });
            
            context.Set<UniswapChainConfiguration>().Add(new UniswapChainConfiguration
            {
                Name = "Monad",
                ChainId = 143,
                RpcUrl = new Uri("https://lb.drpc.live/monad-mainnet"),
                BlockscoutUrl = new Uri("https://monadscan.com"),
                SmartContractAddresses = new UniswapAddresses
                {
                    PoolFactory = EvmAddress.Create("0x204faca1764b154221e35c0d20abb3c525710498"),
                    MultiCall = EvmAddress.Create("0xcA11bde05977b3631167028862bE2a173976CA11"),
                    PositionManager = EvmAddress.Create("0x7197e214c0b767cfb76fb734ab638e2c192f4e53")
                },
                ProtocolVersion = UniswapProtocolVersion.V3
            });
        }
    }
}