namespace CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;

public class MintPositionOperation : PositionOperation
{
    public int TickLower { get; init; }

    public int TickUpper { get; init; }
}