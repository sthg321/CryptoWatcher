using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services;

public class UniswapChainSynchronizer : IUniswapChainSynchronizer
{
    private readonly IWeb3Factory _web3Factory;
    private readonly IChainLogChunkingStrategy _chunkingStrategy;
    private readonly ICashFlowEventMatcher _eventMatcher;
    private readonly IRepository<UniswapLiquidityPositionCashFlow> _poolPositionCashFlowRepository;

    public UniswapChainSynchronizer(IWeb3Factory web3Factory, IChainLogChunkingStrategy chunkingStrategy,
        ICashFlowEventMatcher eventMatcher, IRepository<UniswapLiquidityPositionCashFlow> poolPositionCashFlowRepository)
    {
        _web3Factory = web3Factory;
        _chunkingStrategy = chunkingStrategy;
        _eventMatcher = eventMatcher;
        _poolPositionCashFlowRepository = poolPositionCashFlowRepository;
    }

    public async Task SynchronizeChainAsync(UniswapChainConfiguration chain, CancellationToken ct = default)
    {
        var web3 = _web3Factory.GetWeb3(chain);

        var lastBlockInBlockChain = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();

        foreach (var (from, to) in _chunkingStrategy.CreateChunks(chain.LastProcessedBlock, lastBlockInBlockChain))
        {
            await _poolPositionCashFlowRepository.UnitOfWork.BeginTransactionAsync(ct);

            await foreach (var cashFlow in _eventMatcher.FetchCashFlowEvents(chain, from, to, ct))
            {
                if (cashFlow.Count != 0)
                {
                    await _poolPositionCashFlowRepository.BulkMergeAsync(cashFlow, ct);
                }
            }

            chain.UpdateLastSynchronizedBlock(to);

            await _poolPositionCashFlowRepository.UnitOfWork.SaveChangesAsync(ct);

            await _poolPositionCashFlowRepository.UnitOfWork.CommitTransactionAsync(ct);
        }
    }
}