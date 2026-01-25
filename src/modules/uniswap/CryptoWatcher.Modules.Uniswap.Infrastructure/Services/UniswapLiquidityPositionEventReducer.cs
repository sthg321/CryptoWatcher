using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services;

public class UniswapLiquidityPositionEventReducer : IUniswapLiquidityPositionEventReducer
{
    private readonly IUniswapPositionEventApplier _eventApplier;

    public UniswapLiquidityPositionEventReducer(
        IUniswapPositionEventApplier eventApplier)
    {
        _eventApplier = eventApplier;
    }

    public async Task<UniswapLiquidityPosition[]> ApplyEventsAsync(UniswapChainConfiguration chainConfiguration,
        EvmAddress walletAddress,
        IReadOnlyCollection<UniswapEvent> uniswapEvents,
        IReadOnlyCollection<UniswapLiquidityPosition> currentPositions,
        CancellationToken ct = default)
    {
        var positionsById = currentPositions.ToDictionary(p => p.PositionId);

        foreach (var uniswapEventGroup in uniswapEvents
                     .OrderBy(@event => @event.Timestamp)
                     .GroupBy(@event => @event.Operation.PositionId))
        {
            positionsById.TryGetValue(uniswapEventGroup.Key, out var liquidityPosition);
            
            foreach (var uniswapEvent in uniswapEventGroup)
            {
                liquidityPosition = await _eventApplier.ApplyOperationToPositionAsync(
                    chainConfiguration,
                    walletAddress,
                    uniswapEvent,
                    liquidityPosition,
                    ct);
            }

            positionsById[uniswapEventGroup.Key] = liquidityPosition!;
        }

        return positionsById.Values.ToArray();
    }
}