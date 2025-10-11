using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services;

public class UniswapChainSynchronizer : IUniswapChainSynchronizer
{
    private readonly IWeb3Factory _web3Factory;
    private readonly IChainLogChunkingStrategy _chunkingStrategy;
    private readonly IUniswapCashFlowBlockRangeSynchronizer _blockRangeSynchronizer;

    public UniswapChainSynchronizer(IWeb3Factory web3Factory, IChainLogChunkingStrategy chunkingStrategy,
        IUniswapCashFlowBlockRangeSynchronizer blockRangeSynchronizer)
    {
        _web3Factory = web3Factory;
        _chunkingStrategy = chunkingStrategy;
        _blockRangeSynchronizer = blockRangeSynchronizer;
    }

    public async Task SynchronizeChainAsync(UniswapChainConfiguration chain, CancellationToken ct = default)
    {
        var web3 = _web3Factory.GetWeb3(chain);

        var lastBlockInBlockChain = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();

        foreach (var (from, to) in _chunkingStrategy.CreateChunks(chain.LastProcessedBlock, lastBlockInBlockChain))
        {
            await _blockRangeSynchronizer.SynchronizeBlockRangeAsync(chain, from, to, true, ct);
        }
    }
}