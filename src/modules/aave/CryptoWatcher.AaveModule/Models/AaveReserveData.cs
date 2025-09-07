using System.Numerics;

namespace CryptoWatcher.AaveModule.Models;

public class AaveReserveData
{
    public AaveNetwork Network { get; set; } = null!;
    
    public string AssetAddress { get; init; } = null!;
    
    public BigInteger LiquidityIndex { get; init; }
    
    public BigInteger VariableBorrowIndex { get; init; }
}