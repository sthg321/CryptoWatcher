using CryptoWatcher.Abstractions;
using CryptoWatcher.Infrastructure;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.UniswapModule.Entities;
using Microsoft.EntityFrameworkCore;

namespace CryptoWatcher.Host.Extensions;

public static class DatabaseExtensions
{
    public static void AddConfiguredDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<CryptoWatcherDbContext>());
        services.AddDbContext<CryptoWatcherDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseNpgsql(connectionString);
            optionsBuilder.UseSeeding((context, b) =>
            {
                if (!context.Set<UniswapNetwork>().Any())
                {
                    context.Set<UniswapNetwork>().Add(new UniswapNetwork
                    {
                        Name = "Unichain",
                        RpcUrl = "https://mainnet.unichain.org",
                        NftManagerAddress = "0x4529a01c7a0410167c5740c487a8de60232617bf",
                        PoolFactoryAddress = "0x1f98400000000000000000000000000000000004",
                        MultiCallAddress = "0xb7610f9b733e7d45184be3a1bc966960ccc54f0b",
                        ProtocolVersion = UniswapProtocolVersion.V4
                    });
                }
                
                if (!context.Set<Wallet>().Any())
                {
                    context.Set<Wallet>().Add(new Wallet
                    {
                        Address = "0xeb9191d780c0aB6Ab320C5F05E41ebF81f14255f"
                    });
                }
                
                context.SaveChanges();
            });
        });
    }
}