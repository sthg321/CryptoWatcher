using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.UniswapModule.Models;

namespace CryptoWatcher.Modules.Uniswap.Abstractions;

public interface IUnichainEventFetcher
{
    IAsyncEnumerable<List<LiquidityPoolPositionEvent>> FetchLiquidityPoolEvents(UniswapChainConfiguration chain,
        ulong fromBlock,
        ulong toBlock,
        CancellationToken ct = default);
}
