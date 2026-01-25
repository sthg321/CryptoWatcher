using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Services;

public class UniswapWalletSyncFacade
{
    private readonly IUniswapWalletEventSynchronizer _synchronizer;
    private readonly IRepository<UniswapLiquidityPosition> _positionsRepository;
    private readonly IRepository<UniswapSynchronizationState> _stateRepository;

    public UniswapWalletSyncFacade(IUniswapWalletEventSynchronizer synchronizer,
        IRepository<UniswapLiquidityPosition> positionsRepository,
        IRepository<UniswapSynchronizationState> stateRepository)
    {
        _synchronizer = synchronizer;
        _positionsRepository = positionsRepository;
        _stateRepository = stateRepository;
    }

    public async Task SyncAsync(UniswapChainConfiguration chainConfiguration,
        UniswapSynchronizationState synchronizationState,
        Wallet wallet,
        CancellationToken ct = default)
    {
        await foreach (var batch in _synchronizer.SynchronizeWalletEventsAsync(chainConfiguration, synchronizationState,
                           wallet, ct))
        {
            //fix this
            await _positionsRepository.BulkMergeAsync(batch.UpdatedPositions.ToArray(), ct);

            _stateRepository.Update(synchronizationState);

            await _stateRepository.UnitOfWork.SaveChangesAsync(ct);
        }
    }
}