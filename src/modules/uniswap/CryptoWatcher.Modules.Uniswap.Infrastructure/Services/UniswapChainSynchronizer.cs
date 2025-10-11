using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services;

internal class UniswapChainSynchronizer : IUniswapChainSynchronizer
{
    private readonly IWeb3Factory _web3Factory;
    private readonly IChainLogChunkingStrategy _chunkingStrategy;
    private readonly IUniswapCashFlowBlockRangeSynchronizer _blockRangeSynchronizer;
    private readonly ILogger<UniswapChainSynchronizer> _logger;

    public UniswapChainSynchronizer(IWeb3Factory web3Factory, IChainLogChunkingStrategy chunkingStrategy,
        IUniswapCashFlowBlockRangeSynchronizer blockRangeSynchronizer, ILogger<UniswapChainSynchronizer> logger)
    {
        _web3Factory = web3Factory;
        _chunkingStrategy = chunkingStrategy;
        _blockRangeSynchronizer = blockRangeSynchronizer;
        _logger = logger;
    }

    public async Task SynchronizeChainAsync(UniswapChainConfiguration chain, CancellationToken ct = default)
    {
        var web3 = _web3Factory.GetWeb3(chain);

        var lastBlockInBlockChain = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();

        foreach (var (from, to) in _chunkingStrategy.CreateChunks(chain.LastProcessedBlock, lastBlockInBlockChain))
        {
            _logger.LogInformation("Synchronizing block range {FromBlock} - {ToBlock}", from, to);
            
            await _blockRangeSynchronizer.SynchronizeBlockRangeAsync(chain, from, to, ct);
            
            _logger.LogInformation("Block range {FromBlock} - {ToBlock} synchronized", from, to);
        }
    }
}