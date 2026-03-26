using CryptoWatcher.Modules.Fluid.Abstractions;
using CryptoWatcher.Modules.Fluid.Application.Features;
using CryptoWatcher.Modules.Fluid.Application.Features.LendPositionsSynchronization.Abstractions;
using CryptoWatcher.Modules.Fluid.Entities.Supply;
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


        services.AddSingleton<IFluidLendAddressCache, FluidLendAddressCache>();
        services.AddScoped<IFluidLendAddressRepository, FluidLendAddressRepository>();
        services.AddScoped<IFluidTransactionClassifier, FluidTransactionClassifier>();

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