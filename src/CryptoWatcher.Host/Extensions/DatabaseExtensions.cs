using CryptoWatcher.Abstractions;
using CryptoWatcher.Infrastructure;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.Modules.Aave.ValueObjects;
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
            optionsBuilder.UseProjectables();
            optionsBuilder.UseSeeding((context, _) =>
            {
                SeedUniswapChainData(context);

                SeedAaveChainData(context);

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

    private static void SeedUniswapChainData(DbContext context)
    {
        if (!context.Set<UniswapChainConfiguration>().Any())
        {
            context.Set<UniswapChainConfiguration>().Add(new UniswapChainConfiguration
            {
                Name = "Unichain",
                ChainId = 130,
                RpcUrl = new Uri("https://lb.drpc.live/unichain"),
                BlockscoutUrl = new Uri("https://unichain.blockscout.com"),
                SmartContractAddresses = new UniswapAddresses
                {
                    PoolFactory = EvmAddress.Create("0x1f98400000000000000000000000000000000004"),
                    MultiCall = EvmAddress.Create("0xb7610f9b733e7d45184be3a1bc966960ccc54f0b"),
                    PositionManager =
                        EvmAddress.Create("0x4529A01c7A0410167c5740C487A8DE60232617bf"),
                    StateView = EvmAddress.Create("0x86e8631A016F9068C3f085fAF484Ee3F5fDee8f2")
                },
                LastProcessedBlock = 29634140,
                ProtocolVersion = UniswapProtocolVersion.V4
            });
            context.Set<UniswapChainConfiguration>().Add(new UniswapChainConfiguration
            {
                Name = "Arbitrum",
                ChainId = 42161,
                RpcUrl = new Uri("https://lb.drpc.live/arbitrum"),
                BlockscoutUrl = new Uri("https://arbitrum.blockscout.com"),
                SmartContractAddresses = new UniswapAddresses
                {
                    PoolFactory = EvmAddress.Create("0x360e68faccca8ca495c1b759fd9eee466db9fb32"),
                    MultiCall = EvmAddress.Create("0x842eC2c7D803033Edf55E478F461FC547Bc54EB2"),
                    PositionManager = EvmAddress.Create("0xd88F38F930b7952f2DB2432Cb002E7abbF3dD869"),
                    StateView = EvmAddress.Create("0x76Fd297e2D437cd7f76d50F01AfE6160f86e9990")
                },
                LastProcessedBlock = 389191403,
                ProtocolVersion = UniswapProtocolVersion.V4
            });
        }
    }

    private static void SeedAaveChainData(DbContext context)
    {
        if (!context.Set<AaveChainConfiguration>().Any())
        {
            context.Set<AaveChainConfiguration>().Add(new AaveChainConfiguration
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

            context.Set<AaveChainConfiguration>().Add(new AaveChainConfiguration
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

            context.Set<AaveChainConfiguration>().Add(new AaveChainConfiguration
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
        }
    }
}