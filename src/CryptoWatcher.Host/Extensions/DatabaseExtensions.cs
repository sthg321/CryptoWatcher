using CryptoWatcher.Abstractions;
using CryptoWatcher.Infrastructure;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.ValueObjects;
using CryptoWatcher.Shared.Entities;
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
                if (!context.Set<UniswapChainConfiguration>().Any())
                {
                    context.Set<UniswapChainConfiguration>().Add(new UniswapChainConfiguration
                    {
                        Name = "Unichain",
                        RpcUrl = "https://celo-mainnet.infura.io/v3/8556559626d3455da401e9fd058cc591",
                        SmartContractAddresses = new UniswapAddresses
                        {
                            NftManager = "0x4529a01c7a0410167c5740c487a8de60232617bf",
                            PoolFactory = "0x1f98400000000000000000000000000000000004",
                            MultiCall = "0xb7610f9b733e7d45184be3a1bc966960ccc54f0b",
                        },
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