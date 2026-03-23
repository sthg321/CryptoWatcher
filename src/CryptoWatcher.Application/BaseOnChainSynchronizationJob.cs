using CryptoWatcher.Abstractions;
using CryptoWatcher.Shared.Entities;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Application;

public abstract class BaseOnChainSynchronizationJob<TChainConfiguration, TContext>
    where TChainConfiguration : BaseChainConfiguration
{
    // ReSharper disable once StaticMemberInGenericType
    // field for each inheritor
    private static int _isRunning;

    private readonly IRepository<Wallet> _walletRepository;
    private readonly ILogger _logger;

    protected BaseOnChainSynchronizationJob(IRepository<Wallet> walletRepository, ILogger logger)
    {
        _walletRepository = walletRepository;
        _logger = logger;
    }

    public async Task SynchronizeAsync(CancellationToken ct = default)
    {
        var chainSyncName = typeof(TChainConfiguration).Name;

        if (Interlocked.CompareExchange(ref _isRunning, 1, 0) == 1)
        {
            _logger.LogWarning("Synchronization for {ChainConfiguration} is already in progress", chainSyncName);
            return;
        }

        _logger.LogInformation("Starting synchronization for {ChainConfiguration}", chainSyncName);

        try
        {
            var wallets = await _walletRepository.ListAsync(ct);

            var chains = await GetChainConfiguration(ct);

            var context = await CreateContextAsync(ct);

            foreach (var wallet in wallets)
            {
                using var _ = _logger.BeginScope("Wallet: {Wallet}", wallet.Address);

                _logger.LogInformation("Start synchronizing wallet");

                foreach (var chain in chains)
                {
                    using var __ = _logger.BeginScope("Chain: {ChainName}", chain.Name);

                    _logger.LogInformation("Start synchronizing chain");

                    try
                    {
                        await SynchronizeWalletOnChainAsync(chain, wallet, context, ct);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error while synchronizing chain");
                    }

                    _logger.LogInformation("Synchronization completed for chain");
                }

                _logger.LogInformation("Synchronization completed for wallet");
            }
        }
        finally
        {
            _isRunning = 0;
        }
    }

    protected abstract Task<TContext> CreateContextAsync(CancellationToken ct);

    protected abstract Task<TChainConfiguration[]> GetChainConfiguration(CancellationToken ct);

    protected abstract Task SynchronizeWalletOnChainAsync(TChainConfiguration chain, Wallet wallet, TContext context,
        CancellationToken ct);
}