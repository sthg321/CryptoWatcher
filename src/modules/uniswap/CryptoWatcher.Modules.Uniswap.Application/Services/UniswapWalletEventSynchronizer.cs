using System.Runtime.CompilerServices;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Specifications;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Services;

public class UniswapWalletEventSynchronizer : IUniswapWalletEventSynchronizer
{
    private const int ChunkSize = 50;

    private readonly IUniswapWalletEventExtractor _uniswapWalletEventExtractor;
    private readonly IUniswapLiquidityPositionEventReducer _positionEventReducer;
    private readonly IRepository<UniswapLiquidityPosition> _positionsRepository;

    public UniswapWalletEventSynchronizer(IUniswapWalletEventExtractor uniswapWalletEventExtractor,
        IUniswapLiquidityPositionEventReducer positionEventReducer, IRepository<UniswapLiquidityPosition> positionsRepository)
    {
        _uniswapWalletEventExtractor = uniswapWalletEventExtractor;
        _positionEventReducer = positionEventReducer;
        _positionsRepository = positionsRepository;
    }

    public async IAsyncEnumerable<WalletUniswapEventsSyncBatchResult> SynchronizeWalletEventsAsync(
        UniswapChainConfiguration chainConfiguration,
        UniswapSynchronizationState synchronizationState,
        Wallet wallet,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var uniswapEvents = _uniswapWalletEventExtractor
            .ExtractUniswapEventsAsync(chainConfiguration, synchronizationState, wallet.Address, ct);

        await foreach (var uniswapEventBatch in uniswapEvents.Chunk(ChunkSize).WithCancellation(ct))
        {
            if (uniswapEventBatch.Length == 0)
            {
                continue;
            }

            var positionIds = uniswapEventBatch.Select(@event => @event.Operation.PositionId).Distinct().ToArray();

            var currentPositions = await _positionsRepository.ListAsync(new LiquidityPositionByIds(positionIds), ct);

            var updatedPositions = await _positionEventReducer.ApplyEventsAsync(chainConfiguration, wallet.Address,
                uniswapEventBatch, currentPositions, ct);

            yield return new WalletUniswapEventsSyncBatchResult
            {
                UpdatedPositions = updatedPositions,
                LastEvent = uniswapEventBatch.MaxBy(@event => @event.Timestamp)!
            };
        }
    }
}