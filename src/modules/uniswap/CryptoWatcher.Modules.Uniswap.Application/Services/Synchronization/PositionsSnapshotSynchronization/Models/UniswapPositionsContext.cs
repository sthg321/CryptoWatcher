using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsSnapshotSynchronization.Models;

public sealed class UniswapPositionsContext
{
    private IReadOnlyList<UniswapLiquidityPosition> Positions { get; }

    public UniswapPositionsContext(IReadOnlyList<UniswapLiquidityPosition> positions)
    {
        Positions = positions;
    }

    public UniswapLiquidityPosition[] GetPositionsForChain(UniswapChainConfiguration chain)
    {
        return Positions.Where(position =>
                position.NetworkName == chain.Name &&
                position.ProtocolVersion == chain.ProtocolVersion)
            .ToArray();
    }
}