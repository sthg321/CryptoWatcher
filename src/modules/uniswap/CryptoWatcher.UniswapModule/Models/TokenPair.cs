namespace CryptoWatcher.UniswapModule.Models;

/// <summary>
/// Represents a pair of tokens with raw balances (e.g., liquidity or fees).
/// </summary>
public record TokenPair
{
    /// <summary>
    /// The first token in the pair.
    /// </summary>
    public required Token Token0 { get; init; } = null!;

    /// <summary>
    /// The second token in the pair.
    /// </summary>
    public required Token Token1 { get; init; } = null!;
}