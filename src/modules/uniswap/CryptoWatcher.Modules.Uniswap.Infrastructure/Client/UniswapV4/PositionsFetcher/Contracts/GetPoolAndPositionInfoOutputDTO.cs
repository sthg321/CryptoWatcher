using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace UniswapClient.UniswapV4.PositionsFetcher.Contracts;

[FunctionOutput]
internal class GetPoolAndPositionInfoOutputDTO : IFunctionOutputDTO
{
    [Parameter("tuple", "poolKey", 1)] public PoolKey PoolKey { get; set; } = null!;

    [Parameter("uint256", "info", 2)] public BigInteger PositionInfo { get; set; }
}