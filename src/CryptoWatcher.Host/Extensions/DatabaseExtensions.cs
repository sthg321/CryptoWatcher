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
    public static void AddConfiguredDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<CryptoWatcherDbContext>());
        services.AddDbContext<CryptoWatcherDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseNpgsql(connectionString);
            optionsBuilder.UseSeeding((context, _) =>
            {
                if (!context.Set<UniswapChainConfiguration>().Any())
                {
                    context.Set<UniswapChainConfiguration>().Add(new UniswapChainConfiguration
                    {
                        Name = "Unichain",
                        RpcUrl = new Uri("https://lb.drpc.live/unichain"),
                        BlockscoutUrl = new Uri("https://unichain.blockscout.com"),
                        SmartContractAddresses = new UniswapAddresses
                        {
                            NftManager = EvmAddress.Create("0x4529a01c7a0410167c5740c487a8de60232617bf"),
                            PoolFactory = EvmAddress.Create("0x1f98400000000000000000000000000000000004"),
                            MultiCall = EvmAddress.Create("0xb7610f9b733e7d45184be3a1bc966960ccc54f0b"),
                            PositionManager =
                                EvmAddress.Create("0x4529A01c7A0410167c5740C487A8DE60232617bf"),
                        },
                        LastProcessedBlock = 29634140 ,
                        ProtocolVersion = UniswapProtocolVersion.V4,
                    });

                    context.Set<UniswapChainConfiguration>().Add(new UniswapChainConfiguration
                    {
                        Name = "Arbitrum",
                        RpcUrl = new Uri("https://lb.drpc.live/arbitrum"),
                        BlockscoutUrl = new Uri("https://arbitrum.blockscout.com"),
                        SmartContractAddresses = new UniswapAddresses
                        {
                            NftManager = EvmAddress.Create("0x4529a01c7a0410167c5740c487a8de60232617bf"),
                            PoolFactory = EvmAddress.Create("0x1f98400000000000000000000000000000000004"),
                            MultiCall = EvmAddress.Create("0xb7610f9b733e7d45184be3a1bc966960ccc54f0b"),
                            PositionManager = EvmAddress.Create("0x360E68faCcca8cA495c1B759Fd9EEe466db9FB32")
                        },
                        LastProcessedBlock = 389191403,
                        ProtocolVersion = UniswapProtocolVersion.V4,
                    });
                }

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