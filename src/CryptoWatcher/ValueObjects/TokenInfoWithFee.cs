using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Shared.ValueObjects;

namespace CryptoWatcher.ValueObjects;

/// <summary>
/// Represents token information with additional fee details.
/// </summary>
public record TokenInfoWithFee
{
    private TokenInfoWithFee()
    {
        
    }
    
    /// <summary>
    /// Token symbol (e.g., "ETH", "USDC").
    /// </summary>
    public string Symbol { get; private init; } = null!;

    /// <summary>
    /// Token amount.
    /// </summary>
    public decimal Amount { get; private init; }

    /// <summary>
    /// Represents the amount of fees associated with the token.
    /// </summary>
    public decimal FeeAmount { get; private init; }
    
    /// <summary>
    /// Token price in USD.
    /// </summary>
    public decimal PriceInUsd { get; private init; }

    /// <summary>
    /// Represents the total monetary value in USD of a token by multiplying its amount with its price in USD.
    /// </summary>
    public decimal AmountInUsd => Amount * PriceInUsd;
    
    public decimal FeeAmountInUsd => FeeAmount * PriceInUsd;

    /// <summary>
    /// Creates a new instance of <see cref="TokenInfoWithFee"/> using the provided token information, fee amount, and USD price.
    /// </summary>
    /// <param name="info">The base token information containing symbol, amount, and price in USD.</param>
    /// <param name="feeAmount">The fee amount associated with the token.</param>
    /// <param name="priceInUsd">The price of the token in USD.</param>
    /// <returns>A new instance of <see cref="TokenInfoWithFee"/> populated with the provided data.</returns>
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
    
    // Factory с validation по event
    public static TokenInfoWithFee CreateForEvent(CashFlowEvent @event, string symbol, decimal amount, decimal feeAmount, decimal priceInUsd)
    {
        if (@event == CashFlowEvent.FeeClaim && feeAmount <= 0)
        {
            throw new InvalidOperationException("FeeAmount must be >0 for FeeClaim");
        }

        if (@event != CashFlowEvent.FeeClaim && feeAmount < 0)
        {
            throw new InvalidOperationException("FeeAmount cannot be negative");
        }

        return new TokenInfoWithFee { Symbol = symbol, Amount = amount, FeeAmount = feeAmount, PriceInUsd = priceInUsd };
    }
}