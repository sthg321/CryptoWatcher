using System.Numerics;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.
    Models.PositionEvents;

public class DecreaseLiquidityEvent : PositionEvent
{
    public BigInteger Commission0 { get; init; }

    public BigInteger Commission1 { get; init; }
 
    public bool IsPositionClosed { get; set; }
    
    public new Token? Token0 { get; init; } 

    public new Token? Token1 { get; init; } 

    public bool IsPositionClosedWithOneTokenOnly() => Token0 is null || Token1 is null;
}