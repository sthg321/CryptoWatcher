using CryptoWatcher.Modules.Fluid.Abstractions;
using CryptoWatcher.Modules.Fluid.Application.Features;
using CryptoWatcher.Modules.Fluid.Application.Features.Abstractions;
using CryptoWatcher.Modules.Fluid.Application.Features.LendPositionsSynchronization;
using CryptoWatcher.Modules.Fluid.Application.Features.LendPositionsSynchronization.Abstractions;
using CryptoWatcher.Modules.Fluid.Entities.Supply;
using CryptoWatcher.Modules.Fluid.Infrastructure.Integrations;
using CryptoWatcher.Modules.Fluid.Infrastructure.Integrations.Blockchain;
using CryptoWatcher.Modules.Fluid.Infrastructure.Integrations.Blockchain.Abstractions;
using CryptoWatcher.Modules.Fluid.Infrastructure.Integrations.Blockchain.Events;
using CryptoWatcher.Modules.Fluid.Infrastructure.Integrations.Kafka;
using CryptoWatcher.Modules.Fluid.Infrastructure.Persistence;
using CryptoWatcher.Modules.Fluid.Infrastructure.Persistence.Repositories;
using CryptoWatcher.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoWatcher.Modules.Fluid.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFluidModule(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<FluidDbContext>(options => options
            .UseSeeding((context, _) => SeedData((FluidDbContext)context))
            .UseNpgsql(connectionString, npgsql => npgsql
                .MigrationsHistoryTable("__EFMigrationsHistory", "fluid")
                .MigrationsAssembly(typeof(FluidDbContext).Assembly.FullName)));

        services.AddScoped<IFluidLendAddressRepository, FluidLendAddressRepository>();
        services.AddScoped<IFluidLendPositionRepository, FluidLendPositionRepository>();
        
        services.AddSingleton<IFluidLendAddressCache, FluidLendAddressCache>();
        services.AddSingleton<IFluidTransactionClassifier, FluidTransactionClassifier>();
        services.AddScoped<IFluidGateway, FluidGateway>();
        services.AddScoped<FluidDepositEventHandler>();
        services.AddScoped<IFluidTransactionConsumer, FluidTransactionConsumer>();
        services.AddScoped<IFluidTransactionEnricher, FluidTransactionEnricher>();
        
        services.AddSingleton<IFluidTransactionLogDecoderFactory, FluidTransactionLogDecoderFactory>();
        services.AddSingleton<IFluidTransactionLogDecoder, DepositLogDecoder>();
        services.AddSingleton<IFluidTransactionLogDecoder, WithdrawLogDecoder>();

        services.AddHostedService<FluidBlockchainTransactionTransactionsConsumer>();

        return services;
    }

    private static void SeedData(FluidDbContext context)
    {
        if (!context.FluidLendAddresses.Any())
        {
            context.FluidLendAddresses.AddRange(new FluidLendAddress
            {
                Address = EvmAddress.Create("0x1A996cb54bb95462040408C06122D45D6Cdb6096"),
                ChainId = 42161
            });
        }
    }
}


