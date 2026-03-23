using CryptoWatcher.Abstractions;
using CryptoWatcher.Infrastructure;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.ValueObjects;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CryptoWatcher.Host.Extensions;

public static class DatabaseExtensions
{
    public static void AddConfiguredDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<CryptoWatcherDbContext>());
        
        services.AddDbContext<CryptoWatcherDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("Postgres"));
            optionsBuilder.UseProjectables();
            optionsBuilder.UseSeeding((context, _) =>
            {
                if (!context.Set<Wallet>().Any())
                {
                    context.Set<Wallet>().Add(new Wallet
                    {
                        Address = EvmAddress.Create("0xeb9191d780c0aB6Ab320C5F05E41ebF81f14255f")
                    });
                }

                context.SaveChanges();
            });
        });
    }
}