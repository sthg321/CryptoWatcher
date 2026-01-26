using System.Threading.RateLimiting;
using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions.OperationReaders;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions.Reports;
using CryptoWatcher.Modules.Uniswap.Application.Services;
using CryptoWatcher.Modules.Uniswap.Application.Services.Reports;
using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.OperationReaders;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV3;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV3.LiquidityPool;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV3.LiquidityPoolFactory;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV3.PositionsFetcher;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV4;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV4.LiquidityPool;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV4.PositionsFetcher;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV4.StateView;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV4.UniswapAppApiClient;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Blockchain;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Blockchain.Api;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Etherscan;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Etherscan.Api;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Services;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Services.PositionsSynchronization;
using CryptoWatcher.Modules.Uniswap.Infrastructure.UniswapV3.Abstractions;
using CryptoWatcher.Modules.Uniswap.Infrastructure.UniswapV3.LogEventDecoders;
using CryptoWatcher.Modules.Uniswap.Infrastructure.UniswapV3.Services;
using Microsoft.Extensions.DependencyInjection;
using Nethereum.ABI.ABIDeserialisation;
using Polly;
using Refit;

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

        services.AddScoped<IDailyPositionPerformanceSynchronizer, UniswapDailyPositionPerformanceSynchronizer>();
        
        services.AddSingleton<IUniswapProvider, UniswapProvider>();

        services.AddSingleton<IWeb3Factory, Web3Factory>();

        services.AddScoped<IUniswapOverallReportService, UniswapOverallReportService>();

        services.AddSingleton<IBlockchainGateway, Web3BlockchainGateway>();

        services.AddScoped<IWalletTransactionGateway, EtherscanTransactionGateway>();
        services.AddScoped<IWalletTransactionPaginator, WalletTransactionPaginator>();
        services.AddScoped<IUnprocessedWalletTransactions, UnprocessedWalletTransactions>();
        services.AddScoped<IUniswapTransactionEnricher, UniswapTransactionEnricher>();
        services.AddScoped<IUniswapWalletTransactionScanner, UniswapWalletTransactionScanner>();
        services.AddScoped<IUniswapWalletEventSynchronizer, UniswapWalletEventSynchronizer>();
        services.AddScoped<IUniswapWalletSyncStore, UniswapWalletSyncStore>();
        services.AddScoped<IUniswapWalletSyncOrchestrator, UniswapWalletSyncOrchestrator>();

        services.AddScoped<IUniswapTransactionEventSource, UniswapTransactionEventSource>();

        services.AddRefitClient<IEtherscanApi>()
            .ConfigureHttpClient(client => client.BaseAddress = new Uri("https://api.etherscan.io"));

        services.AddSingleton<IEtherscanApiKeyProvider, EtherscanApiKeyProvider>();

        services.AddSingleton<IBlockchainGateway, Web3BlockchainGateway>();
        services.AddSingleton<IWeb3BlockchainApi, Web3BlockchainApi>();
        services.AddScoped<IUniswapTransactionEventSource, UniswapTransactionEventSource>();
        services.AddScoped<IUniswapSingleTransactionOrchestrator, UniswapSingleTransactionOrchestrator>();

        //v3
        services.AddSingleton<UniswapV3Client>();
        services.AddSingleton<IUniswapV3LiquidityPool, UniswapV3LiquidityPool>();
        services.AddSingleton<IUniswapV3PoolFactory, UniswapV3PoolFactory>();
        services.AddSingleton<IUniswapV3PositionFetcher, UniswapV3PositionFetcher>();

        services.AddSingleton<IPositionOperationsSource, UniswapV3PositionOperationsSource>();
        services.AddSingleton<IUniswapTransactionFilter, UniswapV3TransactionFilter>();
        services.AddSingleton<IUniswapTransactionLogsDecoderFactory, UniswapV3TransactionLogsDecoderFactory>();
        services.AddSingleton<IUniswapPositionEventApplier, UniswapV3PositionEventApplier>();

        services.AddSingleton<IPositionOperationApplierFactory, PositionOperationApplierFactory>();
        services.AddSingleton<IUniswapLiquidityPositionEventReducer, UniswapLiquidityPositionEventReducer>();
        services.AddSingleton<IMintPositionOperationApplier, MintPositionOperationApplier>();
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
        services.AddHttpClient<UniswapAppApiClient>(client =>
            client.BaseAddress = new Uri("https://interface.gateway.uniswap.org")).AddStandardHedgingHandler();

        services.AddKeyedScoped<IPlatformDailyReportDataProvider, UniswapDailyReportService>(UniswapModuleKeyedService
            .DailyPlatformKeyService);
        services.AddSingleton<IUniswapMath, UniswapMath>();
        services.AddScoped<IUniswapPositionsSyncService, UniswapPositionsSyncService>();


        return services;
    }

    private static IServiceCollection AddUniswapOperationAppliers(this IServiceCollection services)
    {
        // Keyed generics с factory func
        RegisterKeyedApplier<CollectFeesOperation, CollectFeesOperationApplier>(services);
        RegisterKeyedApplier<IncreaseLiquidityOperation, IncreaseLiquidityOperationApplier>(services);
        RegisterKeyedApplier<DecreaseLiquidityOperation, DecreaseLiquidityOperationApplier>(services);

        // Фабрика
        services.AddScoped<PositionOperationApplierFactory>();

        return services;
    }

    private static void RegisterKeyedApplier<TOperation, TApplier>(IServiceCollection services)
        where TOperation : PositionOperation
        where TApplier : class, IPositionMutationOperation
    {
        services.AddSingleton<TApplier>();

        var key = typeof(TOperation);
        services.AddKeyedSingleton<IPositionMutationOperation>(key, (sp, _) => sp.GetRequiredService<TApplier>());
    }
}