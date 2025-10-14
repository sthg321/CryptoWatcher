using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IUniswapChainSynchronizer
{
    Task SynchronizeChainAsync(UniswapChainConfiguration chain, CancellationToken ct = default);
}