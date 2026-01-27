using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.PositionEvents;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IUniswapPositionEventApplier
{
    Task<UniswapLiquidityPosition> ApplyOperationToPositionAsync(UniswapChainConfiguration chainConfiguration,
        EvmAddress walletAddress,
        UniswapPositionEvent positionEvent,
        UniswapLiquidityPosition? position,
        CancellationToken ct = default);
}