using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.PositionEvents;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization;

public class UniswapTransactionEnricher : IUniswapTransactionEnricher
{
    private readonly IPositionEventSource _positionEventSource;
    private readonly IUniswapTransactionFilter _uniswapTransactionFilter;

    public UniswapTransactionEnricher(IPositionEventSource positionEventSource,
        IUniswapTransactionFilter uniswapTransactionFilter)
    {
        _positionEventSource = positionEventSource;
        _uniswapTransactionFilter = uniswapTransactionFilter;
    }

    public async Task<UniswapPositionEvent?> TryEnrichAsync(UniswapChainConfiguration chainConfiguration,
        BlockchainTransaction transaction,
        CancellationToken ct = default)
    {
        if (!_uniswapTransactionFilter.IsRelevant(chainConfiguration, transaction))
        {
            return null;
        }

        var uniswapEvent =
            await _positionEventSource.GetEventFromTransactionAsync(chainConfiguration, transaction.Hash, ct);

        // for case when multicall is not a liquidity operation
        if (uniswapEvent is null)
        {
            return null;
        }

        return new UniswapPositionEvent
        {
            Event = uniswapEvent,
            Timestamp = transaction.Timestamp
        };
    }
}