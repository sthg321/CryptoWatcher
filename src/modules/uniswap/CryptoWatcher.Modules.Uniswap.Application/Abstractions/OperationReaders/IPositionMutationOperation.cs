using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions.OperationReaders;

public interface IPositionMutationOperation
{
    Task<UniswapLiquidityPosition> ApplyOperationAsync(
        UniswapChainConfiguration chainConfiguration,
        UniswapLiquidityPosition position,
        PositionOperation operation,
        DateTime timestamp,
        CancellationToken ct = default);
}