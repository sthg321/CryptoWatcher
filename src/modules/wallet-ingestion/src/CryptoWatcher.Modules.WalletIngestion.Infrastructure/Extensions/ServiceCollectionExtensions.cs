using CryptoWatcher.Modules.WalletIngestion.Application.Abstractions;
using CryptoWatcher.Modules.WalletIngestion.Application.Services;
using CryptoWatcher.Modules.WalletIngestion.Infrastructure.Integrations.Configs;
using CryptoWatcher.Modules.WalletIngestion.Infrastructure.Integrations.Etherscan;
using CryptoWatcher.Modules.WalletIngestion.Infrastructure.Integrations.Etherscan.Api;
using CryptoWatcher.Modules.WalletIngestion.Infrastructure.Integrations.Kafka;
using CryptoWatcher.Modules.WalletIngestion.Infrastructure.Persistence;
using CryptoWatcher.Modules.WalletIngestion.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;

namespace CryptoWatcher.Modules.WalletIngestion.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWalletIngestionModule(this IServiceCollection services,
        IConfiguration configuration,
        string connectionString)
    {
        services.AddSingleton<KafkaConfig>(_ =>
        {
            var config = new KafkaConfig();
            configuration.GetSection(nameof(KafkaConfig)).Bind(config);

            return config;
        });

        services.AddDbContext<WalletIngestionDbContext>(builder => builder
            .UseNpgsql(connectionString, npgsql => npgsql
                .MigrationsHistoryTable("__EFMigrationsHistory", "wallet_ingestion")
                .MigrationsAssembly(typeof(WalletIngestionDbContext).Assembly.FullName))
        );

        services.AddScoped<IWalletCheckpointRepository, WalletCheckpointRepository>();
        services.AddScoped<IWalletTransactionGateway, EtherscanTransactionGateway>();
        services.AddScoped<IWalletTransactionPaginator, WalletTransactionPaginator>();
        services.AddScoped<IUnprocessedWalletTransactions, UnprocessedWalletTransactions>();
        services.AddScoped<IWalletTransactionIngestionService, WalletTransactionIngestionService>();
        services.AddScoped<IWalletTransactionIngestionOrchestrator, WalletTransactionIngestionOrchestrator>();

        services.AddRefitClient<IEtherscanApi>()
            .ConfigureHttpClient(client => client.BaseAddress = new Uri("https://api.etherscan.io"));

        services.AddSingleton<IEtherscanApiKeyProvider, EtherscanApiKeyProvider>();

        services.AddSingleton<IWalletTransactionProducer, WalletTransactionProducer>();
        return services;
    }
}