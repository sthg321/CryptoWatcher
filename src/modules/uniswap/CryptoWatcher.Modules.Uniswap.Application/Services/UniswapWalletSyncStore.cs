using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Services;

public class UniswapWalletSyncStore : IUniswapWalletSyncStore
{
    private readonly IRepository<UniswapLiquidityPosition> _positionsRepository;
    private readonly IRepository<UniswapSynchronizationState> _synchronizationStateRepository;
    private readonly TimeProvider _timeProvider;

    public UniswapWalletSyncStore(IRepository<UniswapLiquidityPosition> positionsRepository,
        IRepository<UniswapSynchronizationState> synchronizationStateRepository, TimeProvider timeProvider)
    {
        _positionsRepository = positionsRepository;
        _synchronizationStateRepository = synchronizationStateRepository;
        _timeProvider = timeProvider;
    }

    public async Task SaveWalletSyncBatchAsync(UniswapSynchronizationState state,
        WalletUniswapEventsSyncBatchResult batch, CancellationToken ct = default)
    {
        await _positionsRepository.BulkMergeAsync(batch.UpdatedPositions, ct);

        var lastTransactionHash = batch.LastEvent.Operation.TransactionHash;
        var blockNumber = batch.LastEvent.Operation.BlockNumber;

        state.UpdateLastSynchronizedTransaction(lastTransactionHash, blockNumber, _timeProvider);
        _synchronizationStateRepository.Update(state);
    }
}