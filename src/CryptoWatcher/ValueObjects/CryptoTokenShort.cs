namespace CryptoWatcher.ValueObjects;

public class CryptoTokenShort
{
    /// <summary>
    /// Token address on chain.
    /// </summary>
    public required EvmAddress Address { get; init; } = null!;

    /// <summary>
    /// Token symbol (e.g., "ETH", "USDC").
    /// </summary>
    public required string Symbol { get; init; } = null!;
}