namespace CryptoWatcher.UniswapModule.Entities;

public class TokenInfoWithFee
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
    /// 
    /// </summary>
    public decimal FeeAmount { get; init; }
    
    /// <summary>
    /// Token price in USD.
    /// </summary>
    public decimal PriceInUsd { get; init; }
    
    public decimal AmountInUsd => Amount * PriceInUsd;

    public static TokenInfoWithFee Create(TokenInfo info, decimal feeAmount, decimal priceInUsd)    
    {
        return new TokenInfoWithFee
        {
            Symbol = info.Symbol,
            Amount = info.Amount,
            FeeAmount = feeAmount,
            PriceInUsd = priceInUsd
        };
    }
}