namespace CryptoWatcher.Entities;

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
    public decimal FeeAmount { get; set; }
    
    /// <summary>
    /// Token price in USD.
    /// </summary>
    public decimal PriceInUsd { get; init; }
    
    public decimal AmountInUsd => Amount * PriceInUsd;

    public static TokenInfoWithFee Create(TokenInfo info, decimal feeAmount)
    {
        return new TokenInfoWithFee
        {
            Symbol = info.Symbol,
            Amount = info.Amount,
            FeeAmount = feeAmount,
            PriceInUsd = info.PriceInUsd
        };
    }
}