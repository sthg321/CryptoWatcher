using System.Numerics;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IUniswapCashFlowBlockRangeSynchronizer
{
    Task SynchronizeBlockRangeAsync(UniswapChainConfiguration chain, BigInteger fromBlock,
        BigInteger toBlock, bool updateLastProcessBlock, CancellationToken ct = default);
}