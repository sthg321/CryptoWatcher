using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IUniswapPositionEventApplier
{
    Task<UniswapLiquidityPosition> ApplyOperationToPositionAsync(UniswapChainConfiguration chainConfiguration,
        EvmAddress walletAddress,
        UniswapEvent @event,
        UniswapLiquidityPosition? position,
        CancellationToken ct = default);
}