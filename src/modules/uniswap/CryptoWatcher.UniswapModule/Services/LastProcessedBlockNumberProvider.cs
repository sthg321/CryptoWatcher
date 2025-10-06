namespace CryptoWatcher.UniswapModule.Services;

public interface ILastProcessedBlockNumberProvider
{
    ValueTask<ulong> GetLastProcessedBlockNumberAsync(CancellationToken ct = default);
}

public class LastProcessedBlockNumberProvider : ILastProcessedBlockNumberProvider
{
    public ValueTask<ulong> GetLastProcessedBlockNumberAsync(CancellationToken ct = default)
    {
        return ValueTask.FromResult<ulong>(28813747);
    }
}