namespace CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;

public class UniswapEvent
{
    public PositionOperation Operation { get; init; } = null!;

    public DateTime Timestamp { get; init; }
}