using System.Numerics;
using CryptoWatcher.Abstractions.CacheFlows;
using CryptoWatcher.Shared.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Models;

public class LiquidityPoolPositionEvent
{
    public required string WalletAddress { get; init; } = null!;
    
    public required string TransactionHash { get; init; } = null!;

    public required BigInteger TickLower { get; init; }

    public required BigInteger TickUpper { get; init; }

    public required BigInteger LiquidityDelta { get; init; }
    
    public required DateTimeOffset TimeStamp { get; init; }

    public TokenPair TokenPair { get; init; } = null!;
    
    public CacheFlowEvent Event => DetectEvent();

    private CacheFlowEvent DetectEvent()
    {
        if (LiquidityDelta == 0)
        {
            return CacheFlowEvent.FeeClaim;
        }

        if (LiquidityDelta > 0)
        {
            return CacheFlowEvent.Deposit;
        }

        return CacheFlowEvent.Withdrawal;
    }
}