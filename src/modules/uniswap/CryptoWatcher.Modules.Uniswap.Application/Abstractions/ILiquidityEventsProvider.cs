using System.Numerics;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Models;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface ILiquidityEventsProvider
{
    IAsyncEnumerable<IReadOnlyCollection<LiquidityPoolPositionEvent>> FetchLiquidityPoolEvents(
        UniswapChainConfiguration chain,
        BigInteger fromBlock,
        BigInteger toBlock,
        CancellationToken ct = default);
}
