namespace CryptoWatcher.Abstractions.CacheFlows;

public interface IUsdCashFlow : ICashFlow
{
    decimal Usd { get; init; }
}