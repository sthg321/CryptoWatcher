namespace CryptoWatcher.Entities;

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
    
    public decimal AmountInUsd => Amount * PriceInUsd;
}