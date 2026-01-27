namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.PositionEvents;

public class UniswapPositionEvent
{
    public PositionEvent Event { get; init; } = null!;

    public DateTime Timestamp { get; init; }
}