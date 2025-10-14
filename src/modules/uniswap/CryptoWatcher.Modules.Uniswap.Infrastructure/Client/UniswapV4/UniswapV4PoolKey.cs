namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV4;

internal class UniswapV4PoolKey
{
    public string Currency0 { get; init; } = null!;

    public string Currency1 { get; init; } = null!;

    public uint Fee { get; init; }

    public int TickSpacing { get; init; }

    public string? Hooks { get; init; }
}