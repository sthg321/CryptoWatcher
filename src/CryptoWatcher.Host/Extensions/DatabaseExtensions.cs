using CryptoWatcher.Abstractions;
using CryptoWatcher.Infrastructure;
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
            // optionsBuilder.UseSeeding((context, b) =>
            // {
            //     if (!context.Set<UniswapNetwork>().Any())
            //     {
            //         context.Set<UniswapNetwork>().Add(new UniswapNetwork
            //         {
            //             Name = "ZkSync",
            //             RpcUrl = "https://mainnet.era.zksync.io",
            //             NftManagerAddress = "0x7581a80c84d7488be276e6c7b4c1206f25946502",
            //             PoolFactoryAddress = "0x9D63d318143cF14FF05f8AAA7491904A494e6f13",
            //             MultiCallAddress = "0xb1f9b5fcd56122cdfd7086e017ec63e50dc075e7",
            //             ProtocolVersion = UniswapProtocolVersion.V3
            //         });
            //     
            //         context.Set<UniswapNetwork>().Add(new UniswapNetwork
            //         {
            //             Name = "Unichain",
            //             RpcUrl = "https://mainnet.unichain.org",
            //             NftManagerAddress = "0x4529a01c7a0410167c5740c487a8de60232617bf",
            //             PoolFactoryAddress = "0x1f98400000000000000000000000000000000004",
            //             MultiCallAddress = "0xb7610f9b733e7d45184be3a1bc966960ccc54f0b",
            //             ProtocolVersion = UniswapProtocolVersion.V4
            //         });
            //     }
            //     
            //     if (!context.Set<Wallet>().Any())
            //     {
            //         context.Set<Wallet>().Add(new Wallet
            //         {
            //             Address = "0xeb9191d780c0aB6Ab320C5F05E41ebF81f14255f"
            //         });
            //     }
            //     
            //     context.SaveChanges();
            // });
        });
    }
}