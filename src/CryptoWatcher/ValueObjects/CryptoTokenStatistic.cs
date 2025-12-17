namespace CryptoWatcher.ValueObjects;

public record CryptoTokenStatistic
{
    public decimal Amount { get; init; }

    public decimal PriceInUsd { get; init; }

    public decimal AmountInUsd  => Amount * PriceInUsd;
}