using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Entities;

public class UniswapLiquidityPositionCashFlow : ITokenPairCashFlow
{
    public ulong PositionId { get; init; }

    public string NetworkName { get; init; } = null!;

    public DateTime Date { get; init; }

    public TransactionHash TransactionHash { get; init; } = null!;
    
    public CacheFlowEvent Event { get; init; } = null!;
    
    public TokenInfoWithFee Token0 { get; init; } = null!;

    public TokenInfoWithFee Token1 { get; init; } = null!;

    public static UniswapLiquidityPositionCashFlow CreateFromEvent(CacheFlowEvent @event, ulong positionId, string networkName,
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
        return new TokenInfoWithFee
        {
            Symbol = infoWithAddress.Symbol,
            Amount = @event != CacheFlowEvent.FeeClaim ? infoWithAddress.Amount : 0,
            FeeAmount = @event != CacheFlowEvent.FeeClaim ? 0 : infoWithAddress.Amount,
            PriceInUsd = infoWithAddress.PriceInUsd
        };
    }
}