namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IUniswapChainSynchronizerOrchestrator
{
    Task SynchronizeAllChainsAsync(CancellationToken ct = default);
}