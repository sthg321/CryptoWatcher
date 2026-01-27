using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.UniswapV3.Models.EventLogs;

[Event("Collect")]
public class PoolCollectEventLog
{
    [Parameter("address", "owner", 1, true)]
    public string Owner { get; set; } = null!;

    [Parameter("address", "recipient", 2, false)]
    public string Recipient { get; set; } = null!;

    [Parameter("int24", "tickLower", 3, true)]
    public int TickLower { get; set; }

    [Parameter("int24", "tickUpper", 4, true)]
    public int TickUpper { get; set; }

    [Parameter("uint128", "amount0", 5, false)]
    public BigInteger Amount0 { get; set; }

    [Parameter("uint128", "amount1", 6, false)]
    public BigInteger Amount1 { get; set; }
}