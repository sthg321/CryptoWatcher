using System.Numerics;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Abstractions;

public interface ICashFlowEventMatcher
{
    IAsyncEnumerable<List<UniswapLiquidityPositionCashFlow>> FetchCashFlowEvents(
        UniswapChainConfiguration chainConfiguration,
        BigInteger fromBlock,
        BigInteger toBlock,
        CancellationToken ct = default);
}