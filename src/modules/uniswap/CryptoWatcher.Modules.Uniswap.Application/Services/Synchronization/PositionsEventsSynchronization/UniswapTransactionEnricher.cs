using CryptoWatcher.Modules.Contracts.Messages;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.PositionEvents;
using CryptoWatcher.Modules.Uniswap.Entities;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization;

public class UniswapTransactionEnricher : IUniswapTransactionEnricher
{
    private readonly IPositionEventSource _positionEventSource;
    private readonly IUniswapTransactionFilter _uniswapTransactionFilter;
    private readonly ILogger<UniswapTransactionEnricher> _logger;

    public UniswapTransactionEnricher(IPositionEventSource positionEventSource,
        IUniswapTransactionFilter uniswapTransactionFilter,
        ILogger<UniswapTransactionEnricher> logger)
    {
        _positionEventSource = positionEventSource;
        _uniswapTransactionFilter = uniswapTransactionFilter;
        _logger = logger;
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
        
        if (uniswapEvent is DecreaseLiquidityEvent decrease)
        {
            var liquidityAfter = await _positionEventSource.GetPositionLiquidityAsync(
                chainConfiguration,
                decrease.PositionId);

            decrease.IsPositionClosed = liquidityAfter == 0;
        }
        
        _logger.LogInformation("Enriched transaction with {EventType} for position {PositionId}", uniswapEvent.GetType().Name, uniswapEvent.PositionId);
        
        return new UniswapPositionEvent
        {
            Event = uniswapEvent,
            Timestamp = transaction.Timestamp
        };
    }
}