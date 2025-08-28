namespace CryptoWatcher.Models;

/// <summary>
/// Represents a pair of enriched tokens with additional metadata (symbol, price, etc.).
/// </summary>
public record TokenInfoPair
{
    /// <summary>
    /// The first token with metadata.
    /// </summary>
    public required TokenInfoWithAddress Token0 { get; init; } = null!;

    /// <summary>
    /// The second token with metadata.
    /// </summary>
    public required TokenInfoWithAddress Token1 { get; init; } = null!;
}