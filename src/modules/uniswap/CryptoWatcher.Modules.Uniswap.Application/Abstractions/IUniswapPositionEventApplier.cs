using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.PositionEvents;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IUniswapPositionEventApplier
{
    Task<UniswapLiquidityPosition> ApplyOperationToPositionAsync(UniswapChainConfiguration chainConfiguration,
        UniswapPositionEvent positionEvent,
        UniswapLiquidityPosition? position,
        CancellationToken ct = default);
}