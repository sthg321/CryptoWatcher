namespace CryptoWatcher.UniswapModule.Entities;

public record TokenInfo  
{
    /// <summary>
    /// Token symbol (e.g., "ETH", "USDC").
    /// </summary>
    public string Symbol { get; init; } = null!;

    /// <summary>
    /// Token amount.
    /// </summary>
    public decimal Amount { get; init; }
    
    /// <summary>
    /// Token price in USD.
    /// </summary>
    public decimal PriceInUsd { get; init; }

    /// <summary>
    /// The total value of the token amount in USD, calculated as the product of Amount and PriceInUsd.
    /// </summary>
    public decimal AmountInUsd => Amount * PriceInUsd;
}