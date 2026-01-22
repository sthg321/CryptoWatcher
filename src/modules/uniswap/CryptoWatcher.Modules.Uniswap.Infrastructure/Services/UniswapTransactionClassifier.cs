using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions.OperationReaders;
using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services;

public class UniswapTransactionClassifier : IUniswapTransactionClassifier
{
    private readonly IPositionOperationsSource _positionOperationsSource;
    private readonly IUniswapTransactionPreFilter _uniswapTransactionPreFilter;

    public UniswapTransactionClassifier(IPositionOperationsSource positionOperationsSource,
        IUniswapTransactionPreFilter uniswapTransactionPreFilter)
    {
        _positionOperationsSource = positionOperationsSource;
        _uniswapTransactionPreFilter = uniswapTransactionPreFilter;
    }

    public async Task<PositionOperationInfo?> Classify(UniswapChainConfiguration chainConfiguration,
        BlockchainTransaction transaction,
        CancellationToken ct = default)
    {
        if (!_uniswapTransactionPreFilter.IsRelevant(chainConfiguration, transaction))
        {
            return null;
        }

        var operation =
            await _positionOperationsSource.GetOperationFromTransactionAsync(chainConfiguration, transaction.Hash, ct);

        // for case when multicall is not a liquidity operation
        if (operation is null)
        {
            return null;
        }

        return new PositionOperationInfo
        {
            Operation = operation,
            Timestamp = transaction.Timestamp
        };
    }
}