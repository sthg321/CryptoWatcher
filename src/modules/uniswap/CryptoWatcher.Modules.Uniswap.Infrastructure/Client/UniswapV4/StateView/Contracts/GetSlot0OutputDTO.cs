using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace UniswapClient.UniswapV4.StateView.Contracts;

[FunctionOutput]
public class GetSlot0OutputDTO : IFunctionOutputDTO
{
    [Parameter("uint160", "sqrtPriceX96", 1)]
    public BigInteger SqrtPriceX96 { get; set; }

    [Parameter("int24", "tick", 2)] public int Tick { get; set; }

    [Parameter("uint24", "protocolFee", 3)]
    public uint ProtocolFee { get; set; }

    [Parameter("uint24", "lpFee", 4)] public uint LpFee { get; set; }
}