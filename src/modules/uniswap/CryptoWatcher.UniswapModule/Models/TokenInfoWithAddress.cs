using CryptoWatcher.Entities;

namespace CryptoWatcher.Models;

/// <summary>
/// Contains metadata and pricing information about a token.
/// </summary>
public record TokenInfoWithAddress : TokenInfo
{
    /// <summary>
    /// Token contract address.
    /// </summary>
    public string Address { get; init; } = null!;
}