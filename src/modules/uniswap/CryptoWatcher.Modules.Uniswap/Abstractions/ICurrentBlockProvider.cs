namespace CryptoWatcher.Modules.Uniswap.Abstractions;

public interface ICurrentBlockProvider
{
    ValueTask<long> GetCurrentBlock();
}