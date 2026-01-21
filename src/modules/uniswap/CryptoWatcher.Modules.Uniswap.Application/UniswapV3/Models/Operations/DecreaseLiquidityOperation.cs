using System.Numerics;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;

public class DecreaseLiquidityOperation : PositionOperation
{
    public BigInteger Commission0 { get; init; }
    
    public BigInteger Commission1 { get; init; }

    public bool IsPositionClosed { get; init; }
}