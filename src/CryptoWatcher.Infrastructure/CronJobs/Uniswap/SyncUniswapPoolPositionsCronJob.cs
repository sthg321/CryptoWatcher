using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Services.PositionsSynchronization;
using CryptoWatcher.Shared.Entities;
using Microsoft.Extensions.Logging;
using TickerQ.Utilities.Base;

namespace CryptoWatcher.Infrastructure.CronJobs.Uniswap;

internal class SyncUniswapPoolPositionsCronJob
{
    private readonly IRepository<Wallet> _walletRepository;
    private readonly IRepository<UniswapChainConfiguration> _uniswapNetworkRepository;
    private readonly IUniswapPositionsSyncService _positionsSyncService;
    private readonly ILogger<SyncUniswapPoolPositionsCronJob> _logger;

    public SyncUniswapPoolPositionsCronJob(IRepository<Wallet> walletRepository,
        IRepository<UniswapChainConfiguration> uniswapNetworkRepository,
        IUniswapPositionsSyncService positionsSyncService,
        ILogger<SyncUniswapPoolPositionsCronJob> logger)
    {
        _walletRepository = walletRepository;
        _uniswapNetworkRepository = uniswapNetworkRepository;
        _positionsSyncService = positionsSyncService;
        _logger = logger;
    }

    [TickerFunction(nameof(SyncUniswapPoolPositionsAsync), CronRegistry.Every50Minutes)]
    public async Task SyncUniswapPoolPositionsAsync(CancellationToken ct)
    {
        _logger.StartingPoolSync();

        var wallets = await _walletRepository.ListAsync(ct);
        var networks = await _uniswapNetworkRepository.ListAsync(ct);

        _logger.WalletsAndNetworksCount(wallets.Count, networks.Count);

        var now = DateOnly.FromDateTime(DateTime.Now);

        foreach (var wallet in wallets)
        {
            using var walletScope = _logger.BeginScope("Processing wallet {WalletAddress}", wallet.Address);

            foreach (var network in networks)
            {
                using var networkScope = _logger.BeginScope("Processing uniswapNetwork {NetworkName}", network.Name);

                try
                {
                    await _positionsSyncService.SyncUniswapPositionsAsync(wallet, network, now, ct);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error syncing uniswap pool positions");
                }

                _logger.NetworkProcessingCompleted(network.Name, wallet.Address);
            }

            _logger.WalletProcessingCompleted(wallet.Address);
        }

        _logger.PoolSyncCompleted();
    }
}