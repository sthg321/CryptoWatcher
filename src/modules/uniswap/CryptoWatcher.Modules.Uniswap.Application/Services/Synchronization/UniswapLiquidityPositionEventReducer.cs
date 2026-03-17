using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.PositionEvents;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization;

public class UniswapLiquidityPositionEventReducer : IUniswapLiquidityPositionEventReducer
{
    private readonly IUniswapPositionEventApplier _eventApplier;

    public UniswapLiquidityPositionEventReducer(
        IUniswapPositionEventApplier eventApplier)
    {
        _eventApplier = eventApplier;
    }

    public async Task<UniswapLiquidityPosition[]> ApplyEventsAsync(UniswapChainConfiguration chainConfiguration,
        IReadOnlyCollection<UniswapPositionEvent> uniswapEvents,
        IReadOnlyCollection<UniswapLiquidityPosition> currentPositions,
        CancellationToken ct = default)
    {
        var positionsById = currentPositions.ToDictionary(p => p.PositionId);

        foreach (var uniswapEventGroup in uniswapEvents
                     .OrderBy(@event => @event.Timestamp)
                     .GroupBy(@event => @event.Event.PositionId))
        {
            positionsById.TryGetValue(uniswapEventGroup.Key, out var liquidityPosition);
            
            foreach (var uniswapEvent in uniswapEventGroup)
            {
                liquidityPosition = await _eventApplier.ApplyOperationToPositionAsync(
                    chainConfiguration,
                    uniswapEvent,
                    liquidityPosition,
                    ct);
            }

            positionsById[uniswapEventGroup.Key] = liquidityPosition!;
        }

        return positionsById.Values.ToArray();
    }
}