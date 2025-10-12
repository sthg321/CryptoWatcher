using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Entities;

public class UniswapLiquidityPositionCashFlow : ITokenPairCashFlow
{
    private UniswapLiquidityPositionCashFlow()
    {
    }

    public ulong PositionId { get; init; }

    public string NetworkName { get; init; } = null!;

    public DateTime Date { get; init; }

    public TransactionHash TransactionHash { get; init; } = null!;

    public CacheFlowEvent Event { get; init; } = null!;

    public TokenInfoWithFee Token0 { get; set; } = null!;

    public TokenInfoWithFee Token1 { get; set; } = null!;

    public static UniswapLiquidityPositionCashFlow CreateFromEvent(CacheFlowEvent @event, ulong positionId,
        string networkName,
        string transactionHash,
        TokenInfoPair tokenInfoPair,
        DateTimeOffset timeStamp)
    {
        return new UniswapLiquidityPositionCashFlow
        {
            PositionId = positionId,
            NetworkName = networkName,
            Date = timeStamp.UtcDateTime,
            Event = @event,
            TransactionHash = transactionHash,
            Token0 = CreateFromEvent(tokenInfoPair.Token0, @event),
            Token1 = CreateFromEvent(tokenInfoPair.Token1, @event)
        };
    }

    private static TokenInfoWithFee CreateFromEvent(TokenInfoWithAddress infoWithAddress, CacheFlowEvent @event)
    {
        var amount = @event != CacheFlowEvent.FeeClaim ? infoWithAddress.Amount : 0;
        var feeAmount = @event != CacheFlowEvent.FeeClaim ? 0 : infoWithAddress.Amount;

        return TokenInfoWithFee.CreateForEvent(@event, infoWithAddress.Symbol, amount, feeAmount,
            infoWithAddress.PriceInUsd);
    }
}