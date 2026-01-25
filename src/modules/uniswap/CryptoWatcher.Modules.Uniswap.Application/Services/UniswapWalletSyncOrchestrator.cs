using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Specifications;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Services;

public class UniswapWalletSyncOrchestrator : IUniswapWalletSyncOrchestrator
{
    private readonly IRepository<Wallet> _walletRepository;
    private readonly IRepository<UniswapChainConfiguration> _chainConfigurationRepository;
    private readonly IRepository<UniswapSynchronizationState> _synchronizationStateRepository;
    private readonly IUniswapWalletEventSynchronizer _synchronizer;
    private readonly IUniswapWalletSyncStore _syncStore;

    public UniswapWalletSyncOrchestrator(IRepository<Wallet> walletRepository,
        IRepository<UniswapChainConfiguration> chainConfigurationRepository,
        IRepository<UniswapSynchronizationState> synchronizationStateRepository,
        IUniswapWalletEventSynchronizer synchronizer,
        IUniswapWalletSyncStore syncStore)
    {
        _walletRepository = walletRepository;
        _chainConfigurationRepository = chainConfigurationRepository;
        _synchronizationStateRepository = synchronizationStateRepository;
        _synchronizer = synchronizer;
        _syncStore = syncStore;
    }

    public async Task SyncWalletPositionsAsync(CancellationToken ct = default)
    {
        var wallets = await _walletRepository.ListAsync(ct);

        var chainConfigurations = await _chainConfigurationRepository.ListAsync(ct);

        foreach (var wallet in wallets)
        {
            foreach (var chain in chainConfigurations.Where(x => x.ProtocolVersion == UniswapProtocolVersion.V3))
            {
                var state = await _synchronizationStateRepository.FirstOrDefaultAsync(
                                new UniswapSynchronizationStateByWalletAndChain(chain, wallet), ct) ??
                            new UniswapSynchronizationState(chain, wallet);

                await foreach (var synchronizedPositions in _synchronizer.SynchronizeWalletEventsAsync(chain, state,
                                   wallet, ct))
                {
                    await _syncStore.SaveWalletSyncBatchAsync(state, synchronizedPositions, ct);
                }
            }
        }
    }
}