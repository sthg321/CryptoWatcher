using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsSnapshotSynchronization.Models;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Models;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IPositionEvaluator
{
    Task<PositionValuation> EvaluatePositionAsync(UniswapChainConfiguration chain,
        IUniswapPosition uniswapPosition,
        LiquidityPool pool,
        CancellationToken ct = default);
}