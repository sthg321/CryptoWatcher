using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.PositionEvents;

public class MintPositionEvent : PositionEvent
{
    public EvmAddress PollAddress { get; set; } = null!;
    
    public int TickLower { get; init; }

    public int TickUpper { get; init; }
}