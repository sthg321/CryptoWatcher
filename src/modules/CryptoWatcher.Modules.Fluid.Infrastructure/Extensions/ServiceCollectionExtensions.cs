using CryptoWatcher.Modules.Fluid.Entities.Supply;
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