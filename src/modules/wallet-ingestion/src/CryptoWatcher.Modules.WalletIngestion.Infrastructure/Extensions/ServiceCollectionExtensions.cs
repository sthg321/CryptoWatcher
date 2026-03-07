using CryptoWatcher.Modules.WalletIngestion.Application.Abstractions;
using CryptoWatcher.Modules.WalletIngestion.Application.Services;
using CryptoWatcher.Modules.WalletIngestion.Infrastructure.Integrations.Kafka;
using CryptoWatcher.Modules.WalletIngestion.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoWatcher.Modules.WalletIngestion.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWalletIngestionModule(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<WalletIngestionDbContext>(builder => builder
            .UseNpgsql(connectionString, npgsql => npgsql
                .MigrationsHistoryTable("__EFMigrationsHistory", "wallet_ingestion")
                .MigrationsAssembly(typeof(WalletIngestionDbContext).Assembly.FullName))
        );
        
        services.AddScoped<IWalletTransactionPaginator, WalletTransactionPaginator>();
        services.AddScoped<IUnprocessedWalletTransactions, UnprocessedWalletTransactions>();
        services.AddScoped<IWalletTransactionIngestionService, WalletTransactionIngestionService>();
        services.AddScoped<IWalletTransactionIngestionOrchestrator, WalletTransactionIngestionOrchestrator>();
        services.AddScoped<IWalletTransactionPaginator, WalletTransactionPaginator>();
        
        services.AddSingleton<IWalletTransactionProducer, WalletTransactionProducer>();
        return services;
    }
}