using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions.OperationReaders;
 
public interface IPositionOperationApplier<in TOperation> where TOperation : PositionOperation
{
    Task<UniswapLiquidityPosition> ApplyOperationAsync(
        UniswapChainConfiguration chainConfiguration,
        UniswapLiquidityPosition position,
        TOperation operation,
        DateTime timestamp,
        CancellationToken ct = default);
}