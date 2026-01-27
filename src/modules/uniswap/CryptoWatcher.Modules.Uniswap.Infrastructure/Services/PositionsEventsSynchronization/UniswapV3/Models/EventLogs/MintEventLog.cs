using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.UniswapV3.Models.EventLogs;

[Event("Mint")]
public class MintEventLog
{
    [Parameter("address", "sender", 1, false)]
    public string Sender { get; set; } = null!;

    [Parameter("address", "owner", 1, true)]
    public string Owner { get; set; }= null!;

    [Parameter("int24", "tickLower", 2, true)]
    public int TickLower { get; set; }

    [Parameter("int24", "tickUpper", 3, true)]
    public int TickUpper { get; set; }

    [Parameter("uint128", "amount", 4, false)]
    public BigInteger Amount { get; set; }           // liquidity

    [Parameter("uint256", "amount0", 5, false)]
    public BigInteger Amount0 { get; set; }

    [Parameter("uint256", "amount1", 6, false)]
    public BigInteger Amount1 { get; set; }
}