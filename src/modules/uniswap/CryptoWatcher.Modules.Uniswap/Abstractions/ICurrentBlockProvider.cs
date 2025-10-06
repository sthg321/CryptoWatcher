namespace CryptoWatcher.UniswapModule.Abstractions;

public interface ICurrentBlockProvider
{
    ValueTask<long> GetCurrentBlock();
}