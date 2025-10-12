using System.Numerics;

namespace CryptoWatcher.Modules.Uniswap.Application.Models;

public class LiquidityEventLog
{
    public string Address { get; set; } = null!;

    public string[] Topics { get; set; } = null!;
    
    public BigInteger Data { get; set; }
}