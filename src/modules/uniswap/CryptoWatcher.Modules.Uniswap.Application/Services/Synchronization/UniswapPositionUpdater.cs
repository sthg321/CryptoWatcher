using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models
    .PositionEvents;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Specifications;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization;

public class UniswapPositionUpdater : IUniswapPositionUpdater
{
    private readonly IRepository<UniswapLiquidityPosition> _positionsRepository;
    private readonly IUniswapLiquidityPositionEventReducer _eventReducer;

    public UniswapPositionUpdater(IRepository<UniswapLiquidityPosition> positionsRepository,
        IUniswapLiquidityPositionEventReducer eventReducer)
    {
        _positionsRepository = positionsRepository;
        _eventReducer = eventReducer;
    }

    public async Task<UniswapLiquidityPosition[]> UpdateFromEventAsync(
        UniswapChainConfiguration chain,
        UniswapPositionEvent[] uniswapEvents,
        CancellationToken ct = default)
    {
        var positionIds = uniswapEvents
            .Select(e => e.Event.PositionId)
            .Distinct()
            .ToArray();

        var currentPositions =
            await _positionsRepository.ListAsync(new UniswapLiquidityPositionFullAggregate(chain, positionIds), ct);

        return await _eventReducer.ApplyEventsAsync(
            chain,
            uniswapEvents,
            currentPositions,
            ct);
    }
}