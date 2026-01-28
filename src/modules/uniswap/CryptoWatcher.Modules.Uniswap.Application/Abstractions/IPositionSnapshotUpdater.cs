using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IPositionSnapshotUpdater
{
    IAsyncEnumerable<UniswapLiquidityPosition> GetUpdatedPositionsAsync(UniswapChainConfiguration chain,
        IReadOnlyCollection<UniswapLiquidityPosition> dbPositions,
        DateOnly snapshotDay,
        CancellationToken ct = default);
}