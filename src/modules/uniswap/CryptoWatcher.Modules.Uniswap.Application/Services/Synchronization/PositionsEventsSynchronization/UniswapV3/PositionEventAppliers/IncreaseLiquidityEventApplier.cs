using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.PositionEvents;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.PositionEventAppliers;

public class IncreaseLiquidityEventApplier : BasePositionEventApplier<IncreaseLiquidityEvent>
{
    public IncreaseLiquidityEventApplier(ITokenEnricher tokenEnricher) : base(tokenEnricher)
    {
    }


    protected override ValueTask ApplyOperation(UniswapChainConfiguration chainConfiguration,
        UniswapLiquidityPosition position,
        TokenInfoPair enrichedTokens, IncreaseLiquidityEvent @event, DateTime timestamp)
    {
        position.AddCashFlow(CashFlowEvent.Deposit, enrichedTokens, @event.TransactionHash, timestamp);

        return ValueTask.CompletedTask;
    }
}