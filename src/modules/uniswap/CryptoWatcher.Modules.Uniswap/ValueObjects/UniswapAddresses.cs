using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.ValueObjects;

public class UniswapAddresses
{
    public EvmAddress NftManager { get; init; } = null!;
        
    public EvmAddress PoolFactory { get; init; } = null!;

    public EvmAddress MultiCall { get; init; } = null!;
}