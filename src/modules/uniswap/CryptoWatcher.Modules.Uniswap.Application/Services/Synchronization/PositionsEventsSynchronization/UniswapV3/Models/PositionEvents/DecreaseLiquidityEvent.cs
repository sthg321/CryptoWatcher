using System.Numerics;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.PositionEvents;

public class DecreaseLiquidityEvent : PositionEvent
{
    public BigInteger Commission0 { get; init; }
    
    public BigInteger Commission1 { get; init; }

    public bool IsPositionClosed { get; init; }
}