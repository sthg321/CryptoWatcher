using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.PositionEvents;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization;

public class UniswapPositionUpdater : IUniswapPositionUpdater
{
    private readonly IUniswapLiquidityPositionEventReducer _eventReducer;
    private readonly IUniswapLiquidityPositionRepository _repository;

    public UniswapPositionUpdater(IUniswapLiquidityPositionEventReducer eventReducer,
        IUniswapLiquidityPositionRepository repository)
    {
        _eventReducer = eventReducer;
        _repository = repository;
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

        var currentPositions = await _repository.GetActiveAsync(chain, positionIds, ct);

        return await _eventReducer.ApplyEventsAsync(chain, uniswapEvents, currentPositions, ct);
    }
}