using System.Numerics;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Modules.Uniswap.Application.Services;

public class UniswapCashFlowBlockRangeSynchronizer : IUniswapCashFlowBlockRangeSynchronizer
{
    private readonly ICashFlowEventMatcher _eventMatcher;
    private readonly IRepository<UniswapLiquidityPositionCashFlow> _poolPositionCashFlowRepository;
    private readonly ILogger<UniswapCashFlowBlockRangeSynchronizer> _logger;

    public UniswapCashFlowBlockRangeSynchronizer(
        ICashFlowEventMatcher eventMatcher,
        IRepository<UniswapLiquidityPositionCashFlow> poolPositionCashFlowRepository,
        ILogger<UniswapCashFlowBlockRangeSynchronizer> logger)
    {
        _eventMatcher = eventMatcher;
        _poolPositionCashFlowRepository = poolPositionCashFlowRepository;
        _logger = logger;
    }

    public async Task  SynchronizeBlockRangeAsync(UniswapChainConfiguration chain, BigInteger fromBlock,
        BigInteger toBlock,
        CancellationToken ct = default)
    {
        using var scope = _logger.BeginScope("Synchronizing cash flow events for chain {ChainName}", chain.Name);

        bool isTransactionHolder = false;
        if (!_poolPositionCashFlowRepository.UnitOfWork.HasActiveTransaction)
        {
            await _poolPositionCashFlowRepository.UnitOfWork.BeginTransactionAsync(ct);
            isTransactionHolder = true;
        }

        try
        {
            await foreach (var cashFlowBatch in _eventMatcher.FetchCashFlowEvents(chain, fromBlock, toBlock, ct))
            {
                _logger.LogInformation("Synchronizing {CashFlowEventsCount} cash flow events", cashFlowBatch.Count);

                await _poolPositionCashFlowRepository.BulkMergeAsync(cashFlowBatch, ct);

                _logger.LogInformation("Cash flow events synchronized");
            }

            await _poolPositionCashFlowRepository.UnitOfWork.SaveChangesAsync(ct);
            if (isTransactionHolder)
            {
                await _poolPositionCashFlowRepository.UnitOfWork.CommitTransactionAsync(ct);
            }
        }
        catch
        {
            await _poolPositionCashFlowRepository.UnitOfWork.RollbackTransactionAsync(ct);
            throw;
        }
    }
}