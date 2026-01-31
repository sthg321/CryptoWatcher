using System.Numerics;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.PositionEvents;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.PositionEventAppliers;

public class DecreaseLiquidityEventApplier : BasePositionEventApplier<DecreaseLiquidityEvent>
{
    public DecreaseLiquidityEventApplier(ITokenEnricher tokenEnricher) : base(tokenEnricher)
    {
    }

    protected override async ValueTask ApplyOperation(UniswapChainConfiguration chainConfiguration,
        UniswapLiquidityPosition position,
        TokenInfoPair enrichedTokens, DecreaseLiquidityEvent @event, DateTime timestamp)
    {
        position.AddCashFlow(CashFlowEvent.Withdrawal, enrichedTokens, @event.TransactionHash, timestamp);

        if (@event.Commission0 != 0 || @event.Commission1 != 0)
        {
            var enrichedCommission = await EnrichTokensAsync(chainConfiguration,
                CreateTokenFromPosition(@event.Commission0, position.Token0),
                CreateTokenFromPosition(@event.Commission1, position.Token1));

            position.AddCashFlow(CashFlowEvent.FeeClaim, enrichedCommission, @event.TransactionHash, timestamp);
        }

        if (@event.IsPositionClosed)
        {
            position.ClosePosition(DateOnly.FromDateTime(timestamp));
        }
    }

    private static Token CreateTokenFromPosition(BigInteger amount, CryptoToken cryptoToken) => new()
    {
        Address = cryptoToken.Address,
        Balance = amount
    };
}