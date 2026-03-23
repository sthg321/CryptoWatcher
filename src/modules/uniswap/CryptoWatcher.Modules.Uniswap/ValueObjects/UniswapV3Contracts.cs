using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.ValueObjects;

public class UniswapV3Contracts : IUniswapContracts
{
    public EvmAddress PositionManager { get; init; } = null!;

    public EvmAddress PoolFactory { get; init; } = null!;

    public EvmAddress MultiCall { get; init; } = null!;
}