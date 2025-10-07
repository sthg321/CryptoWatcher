using CryptoWatcher.UniswapModule.Models;

namespace CryptoWatcher.Modules.Uniswap.Abstractions;

public interface IUnichainEventFetcher
{
    IAsyncEnumerable<List<LiquidityPoolPositionEvent>> FetchLiquidityPoolEvents(string unichainRpc,
        ulong fromBlock,
        ulong toBlock,
        CancellationToken ct = default);
}
