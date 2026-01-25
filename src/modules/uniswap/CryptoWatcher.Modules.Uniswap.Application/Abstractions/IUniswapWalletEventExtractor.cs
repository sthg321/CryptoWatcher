using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IUniswapWalletEventExtractor
{
    IAsyncEnumerable<UniswapEvent> ExtractUniswapEventsAsync(UniswapChainConfiguration chain,
        UniswapSynchronizationState synchronizationState, EvmAddress wallet,
        CancellationToken ct = default);
}