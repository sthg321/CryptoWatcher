using System.Numerics;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Services;

public class UniswapCashFlowBlockRangeSynchronizer : IUniswapCashFlowBlockRangeSynchronizer
{
    private readonly ICashFlowEventMatcher _eventMatcher;
    private readonly IRepository<UniswapLiquidityPositionCashFlow> _poolPositionCashFlowRepository;

    public UniswapCashFlowBlockRangeSynchronizer(
        ICashFlowEventMatcher eventMatcher,
        IRepository<UniswapLiquidityPositionCashFlow> poolPositionCashFlowRepository)
    {
        _eventMatcher = eventMatcher;
        _poolPositionCashFlowRepository = poolPositionCashFlowRepository;
    }

    public async Task SynchronizeBlockRangeAsync(UniswapChainConfiguration chain, BigInteger fromBlock,
        BigInteger toBlock, bool updateLastProcessBlock, CancellationToken ct = default)
    {
        await _poolPositionCashFlowRepository.UnitOfWork.BeginTransactionAsync(ct);

        try
        {
            await foreach (var cashFlowBatch in _eventMatcher.FetchCashFlowEvents(chain, fromBlock, toBlock, ct))
            {
                await _poolPositionCashFlowRepository.BulkMergeAsync(cashFlowBatch, ct);
            }

            if (updateLastProcessBlock)
            {
                chain.UpdateLastSynchronizedBlock(toBlock);
            }

            await _poolPositionCashFlowRepository.UnitOfWork.SaveChangesAsync(ct);
            await _poolPositionCashFlowRepository.UnitOfWork.CommitTransactionAsync(ct);
        }
        catch
        {
            await _poolPositionCashFlowRepository.UnitOfWork.RollbackTransactionAsync(ct);
            throw;
        }
    }
}