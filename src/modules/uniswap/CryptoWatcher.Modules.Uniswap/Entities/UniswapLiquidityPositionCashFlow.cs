using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Modules.Uniswap.Models;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Entities;

public class UniswapLiquidityPositionCashFlow : ITokenPairCashFlow
{
    private UniswapLiquidityPositionCashFlow()
    {
    }

    public UniswapLiquidityPositionCashFlow(
        UniswapLiquidityPosition position,
        LiquidityPoolPositionEvent positionEvent,
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
        Token0 = tokenInfoPair.Token0.ToStatistic();
        Token1 = tokenInfoPair.Token0.ToStatistic();
    }

    public UniswapLiquidityPositionCashFlow(ulong positionId, string networkName, DateTime date,
        CashFlowEvent cashFlowEvent, TransactionHash transactionHash, TokenInfoPair infoPair)
    {
        PositionId = positionId;
        NetworkName = networkName;
        Date = date;
        Event = cashFlowEvent;
        TransactionHash = transactionHash;
        Token0 = infoPair.Token0.ToStatistic();
        Token1 = infoPair.Token1.ToStatistic();
    }

    public ulong PositionId { get; init; }

    public string NetworkName { get; init; } = null!;

    public DateTime Date { get; init; }

    public TransactionHash TransactionHash { get; init; } = null!;

    public CashFlowEvent Event { get; init; } = null!;

    public CryptoTokenStatistic Token0 { get; set; } = null!;

    public CryptoTokenStatistic Token1 { get; set; } = null!;

    public decimal FeeInUsd => Event == CashFlowEvent.FeeClaim ? Token0.AmountInUsd + Token1.AmountInUsd : 0;
}