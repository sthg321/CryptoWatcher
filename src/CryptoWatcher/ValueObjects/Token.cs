using System.Numerics;

namespace CryptoWatcher.ValueObjects;

/// <summary>
/// Represents a token with an address and raw balance (e.g., liquidity or fee amount).
/// </summary>
public record Token
{
    /// <summary>
    /// Token contract address.
    /// </summary>
    public EvmAddress Address { get; init; } = null!;

    /// <summary>
    /// Token balance (in raw units, not adjusted for decimals).
    /// </summary>
    public BigInteger Balance { get; init; }
}