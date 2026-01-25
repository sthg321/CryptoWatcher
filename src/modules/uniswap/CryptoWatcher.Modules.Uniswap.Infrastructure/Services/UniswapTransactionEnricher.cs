using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions.OperationReaders;
using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services;

public class UniswapTransactionEnricher : IUniswapTransactionEnricher
{
    private readonly IPositionOperationsSource _positionOperationsSource;
    private readonly IUniswapTransactionFilter _uniswapTransactionFilter;

    public UniswapTransactionEnricher(IPositionOperationsSource positionOperationsSource,
        IUniswapTransactionFilter uniswapTransactionFilter)
    {
        _positionOperationsSource = positionOperationsSource;
        _uniswapTransactionFilter = uniswapTransactionFilter;
    }

    public async Task<UniswapEvent?> TryEnrichAsync(UniswapChainConfiguration chainConfiguration,
        BlockchainTransaction transaction,
        CancellationToken ct = default)
    {
        if (!_uniswapTransactionFilter.IsRelevant(chainConfiguration, transaction))
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

        return new UniswapEvent
        {
            Operation = operation,
            Timestamp = transaction.Timestamp
        };
    }
}