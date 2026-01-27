using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Specifications;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.TransactionSynchronization;

public class UniswapSingleTransactionOrchestrator : IUniswapSingleTransactionOrchestrator
{
    private readonly IUniswapTransactionEventSource _transactionEventSource;
    private readonly IUniswapLiquidityPositionEventReducer _positionEventReducer;
    private readonly IRepository<UniswapLiquidityPosition> _positionsRepository;

    public UniswapSingleTransactionOrchestrator(
        IUniswapTransactionEventSource transactionEventSource,
        IUniswapLiquidityPositionEventReducer positionEventReducer,
        IRepository<UniswapLiquidityPosition> positionsRepository)
    {
        _transactionEventSource = transactionEventSource;
        _positionEventReducer = positionEventReducer;
        _positionsRepository = positionsRepository;
    }

    public async Task SyncTransactionAsync(
        UniswapChainConfiguration chain,
        Wallet wallet,
        TransactionHash transactionHash,
        CancellationToken ct = default)
    {
        var uniswapEvent = await _transactionEventSource.GetUniswapEventAsync(chain, transactionHash, ct);

        if (uniswapEvent is null)
        {
            return;
        }

        var positionId = uniswapEvent.Event.PositionId;

        var currentPosition = await _positionsRepository.FirstOrDefaultAsync(
            new LiquidityPositionByIds(positionId),
            ct);

        var currentPositions = currentPosition is not null
            ? new[] { currentPosition }
            : [];

        var updatedPositions = await _positionEventReducer.ApplyEventsAsync(
            chain,
            wallet.Address,
            [uniswapEvent],
            currentPositions,
            ct);

        await _positionsRepository.BulkMergeAsync(updatedPositions, ct);
    }
}