namespace CryptoWatcher.ValueObjects;

public record CryptoTokenStatisticWithFee : CryptoTokenStatistic
{
    public required decimal Fee { get; init; }

    public decimal FeeInUsd => Fee * PriceInUsd;

    public static CryptoTokenStatisticWithFee From(CryptoToken token, CryptoToken fee)
    {
        return new CryptoTokenStatisticWithFee
        {
            Amount = token.Amount,
            PriceInUsd = token.PriceInUsd,
            Fee = fee.Amount
        };
    }
}