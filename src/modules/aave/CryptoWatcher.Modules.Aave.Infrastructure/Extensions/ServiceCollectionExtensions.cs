using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Application.Abstractions;
using CryptoWatcher.Modules.Aave.Application.Abstractions;
using CryptoWatcher.Modules.Aave.Application.Services;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.Modules.Aave.Infrastructure.Integrations.Blockchain;
using CryptoWatcher.Modules.Aave.Infrastructure.Integrations.Blockchain.UiPoolDataProvider;
using CryptoWatcher.Modules.Aave.Infrastructure.Persistence;
using CryptoWatcher.Modules.Aave.Infrastructure.Persistence.Queries;
using CryptoWatcher.Modules.Aave.Infrastructure.Persistence.Repositories;
using CryptoWatcher.Modules.Aave.ValueObjects;
using CryptoWatcher.ValueObjects;
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

        services.AddDbContext<AaveDbContext>(builder => builder
            .UseNpgsql(connectionString, npgsql => npgsql
                .MigrationsHistoryTable("__EFMigrationsHistory", "aave")
                .MigrationsAssembly(typeof(AaveDbContext).Assembly.FullName))
            .UseProjectables()
            .UseSeeding((context, hasChanges) =>
            {
                if (hasChanges)
                {
                    SeedAaveChainData(context);
                    context.SaveChanges();
                }
            })
        );

        services.AddSingleton<IUiPoolDataProviderFetcher, UiPoolDataProviderFetcher>();

        services.AddSingleton<IAaveGateway, AaveGateway>();
        services.AddSingleton<IAaveHealthFactorCalculator, AaveHealthFactorCalculator>();

        services.AddScoped<IAavePositionRepository, AavePositionRepository>();
        services.AddScoped<IAaveReportQuery, AaveReportQuery>();

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

    private static void SeedAaveChainData(DbContext context)
    {
        if (!context.Set<AaveProtocolConfiguration>().Any())
        {
            context.Set<AaveProtocolConfiguration>().Add(new AaveProtocolConfiguration
            {
                Name = "Avalanche",
                RpcUrl = new Uri("https://lb.drpc.live/avalanche"),
                SmartContractAddresses = new AaveAddresses
                {
                    PoolAddressesProviderAddress =
                        EvmAddress.Create("0xa97684ead0e402dC232d5A977953DF7ECBaB3CDb"),
                    UiPoolDataProviderAddress = EvmAddress.Create("0x50B4a66bF4D41e6252540eA7427D7A933Bc3c088")
                }
            });

            context.Set<AaveProtocolConfiguration>().Add(new AaveProtocolConfiguration
            {
                Name = "Cello",
                RpcUrl = new Uri("https://lb.drpc.live/celo"),
                SmartContractAddresses = new AaveAddresses
                {
                    PoolAddressesProviderAddress =
                        EvmAddress.Create("0x9F7Cf9417D5251C59fE94fB9147feEe1aAd9Cea5"),
                    UiPoolDataProviderAddress = EvmAddress.Create("0xf07fFd12b119b921C4a2ce8d4A13C5d1E3000d6e")
                }
            });

            context.Set<AaveProtocolConfiguration>().Add(new AaveProtocolConfiguration
            {
                Name = "Ink",
                RpcUrl = new Uri("https://lb.drpc.live/ink"),
                SmartContractAddresses = new AaveAddresses
                {
                    PoolAddressesProviderAddress =
                        EvmAddress.Create("0x4172E6aAEC070ACB31aaCE343A58c93E4C70f44D"),
                    UiPoolDataProviderAddress = EvmAddress.Create("0x39bc1bfDa2130d6Bb6DBEfd366939b4c7aa7C697")
                }
            });

            context.Set<AaveProtocolConfiguration>().Add(new AaveProtocolConfiguration
            {
                Name = "Arbitrum",
                RpcUrl = new Uri("https://lb.drpc.live/arbitrum"),
                SmartContractAddresses = new AaveAddresses
                {
                    PoolAddressesProviderAddress =
                        EvmAddress.Create("0xa97684ead0e402dC232d5A977953DF7ECBaB3CDb"),
                    UiPoolDataProviderAddress = EvmAddress.Create("0x145dE30c929a065582da84Cf96F88460dB9745A7")
                }
            });
        }
    }
}