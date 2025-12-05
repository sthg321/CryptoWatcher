using System.Numerics;
using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Models;

public class LiquidityPoolPositionEvent
{
    public required EvmAddress WalletAddress { get; init; } = null!;

    public required TransactionHash TransactionHash { get; init; } = null!;

    public required BigInteger TickLower { get; init; }

    public required BigInteger TickUpper { get; init; }

    public required BigInteger LiquidityDelta { get; init; }

    public required DateTime TimeStamp { get; init; }

    public TokenPair TokenPair { get; init; } = null!;

    public CashFlowEvent Event => DetectEvent();

    private CashFlowEvent DetectEvent()
    {
        if (LiquidityDelta == 0)
        {
            return CashFlowEvent.FeeClaim;
        }

        if (LiquidityDelta > 0)
        {
            return CashFlowEvent.Deposit;
        }

        return CashFlowEvent.Withdrawal;
    }
}