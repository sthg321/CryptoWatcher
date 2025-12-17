namespace CryptoWatcher.ValueObjects;

public record CryptoToken
{
    /// <summary>
    /// Token address on chain.
    /// </summary>
    public required EvmAddress Address { get; init; } = null!;

    /// <summary>
    /// Token symbol (e.g., "ETH", "USDC").
    /// </summary>
    public required string Symbol { get; init; } = null!;

    /// <summary>
    /// Token amount.
    /// </summary>
    public required decimal Amount { get; init; }

    /// <summary>
    /// Token price in USD.
    /// </summary>
    public required decimal PriceInUsd { get; init; }

    /// <summary>
    /// The total value of the token amount in USD, calculated as the product of Amount and PriceInUsd.
    /// </summary>
    public decimal AmountInUsd => Amount * PriceInUsd;

    public CryptoTokenStatistic ToStatistic() => new() { Amount = Amount, PriceInUsd = PriceInUsd };
}