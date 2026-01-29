using CryptoWatcher.Abstractions;
using CryptoWatcher.Application;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Specifications;
using CryptoWatcher.Shared.Entities;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization;

public class UniswapWalletSyncOrchestrator : BaseOnChainSynchronizationJobWithoutContext<UniswapChainConfiguration>,
    IUniswapWalletSyncOrchestrator
{
    private readonly IUniswapWalletEventSynchronizer _synchronizer;
    private readonly IUniswapWalletSyncStore _syncStore;
    private readonly IRepository<UniswapSynchronizationState> _stateRepository;

    public UniswapWalletSyncOrchestrator(
        IRepository<Wallet> walletRepository,
        IRepository<UniswapChainConfiguration> chainRepository,
        ILogger<UniswapWalletSyncOrchestrator> logger,
        IUniswapWalletEventSynchronizer synchronizer, 
        IUniswapWalletSyncStore syncStore,
        IRepository<UniswapSynchronizationState> stateRepository) : base(walletRepository, chainRepository, logger)
    {
        _synchronizer = synchronizer;
        _syncStore = syncStore;
        _stateRepository = stateRepository;
    }

    protected override async Task SynchronizeWalletOnChainAsync(UniswapChainConfiguration chain, Wallet wallet,
        EmptyContext context,
        CancellationToken ct)
    {
        if (chain.ProtocolVersion == UniswapProtocolVersion.V3)
        {
            return;
        }

        var state = await _stateRepository.FirstOrDefaultAsync(
                        new UniswapSynchronizationStateByWalletAndChain(chain, wallet), ct) ??
                    new UniswapSynchronizationState(chain, wallet);

        await foreach (var synchronizedPositions in _synchronizer.SynchronizeWalletEventsAsync(chain, state,
                           wallet, ct))
        {
            await _syncStore.SaveWalletSyncBatchAsync(state, synchronizedPositions, ct);
        }
    }
}