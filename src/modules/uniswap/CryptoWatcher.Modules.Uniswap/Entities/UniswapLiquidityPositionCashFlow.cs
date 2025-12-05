using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Modules.Uniswap.Models;
using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Entities;

public class UniswapLiquidityPositionCashFlow : ITokenPairCashFlow
{
    private UniswapLiquidityPositionCashFlow()
    {
    }

    public UniswapLiquidityPositionCashFlow(
        UniswapLiquidityPosition position,
        LiquidityPoolPositionEvent  positionEvent,
        TokenInfoPair tokenInfoPair,
        DateTime date)
    {
        if (date.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("Only UTC dates are supported.", nameof(date));
        }
        
        PositionId = position.PositionId;
        NetworkName = position.NetworkName;
        Date = date;
        Event = positionEvent.Event;
        TransactionHash = positionEvent.TransactionHash;
        Token0 = CreateFromEvent(tokenInfoPair.Token0, positionEvent.Event);
        Token1 = CreateFromEvent(tokenInfoPair.Token1, positionEvent.Event);
    }

    public ulong PositionId { get; init; }

    public string NetworkName { get; init; } = null!;

    public DateTime Date { get; init; }

    public TransactionHash TransactionHash { get; init; } = null!;

    public CashFlowEvent Event { get; init; } = null!;

    public TokenInfoWithFee Token0 { get; set; } = null!;

    public TokenInfoWithFee Token1 { get; set; } = null!;

    public decimal FeeInUsd => Token0.FeeAmountInUsd + Token1.FeeAmountInUsd;
 
    private static TokenInfoWithFee CreateFromEvent(TokenInfoWithAddress infoWithAddress, CashFlowEvent @event)
    {
        var amount = @event != CashFlowEvent.FeeClaim ? infoWithAddress.Amount : 0;
        var feeAmount = @event != CashFlowEvent.FeeClaim ? 0 : infoWithAddress.Amount;

        return TokenInfoWithFee.CreateForEvent(@event, infoWithAddress.Symbol, amount, feeAmount,
            infoWithAddress.PriceInUsd);
    }
}