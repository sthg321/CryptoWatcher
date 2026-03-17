using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.PositionEvents;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IUniswapLiquidityPositionEventReducer
{
    Task<UniswapLiquidityPosition[]> ApplyEventsAsync(UniswapChainConfiguration chainConfiguration,
        IReadOnlyCollection<UniswapPositionEvent> uniswapEvents,
        IReadOnlyCollection<UniswapLiquidityPosition> currentPositions,
        CancellationToken ct = default);
}