using Nethereum.ABI.FunctionEncoding.Attributes;

namespace UniswapClient.UniswapV3.LiquidityPoolFactory.Contracts;

[FunctionOutput]
internal class GetPairPoolsOutputDto : IFunctionOutputDTO
{
    [Parameter("tuple[]", "", 1)] public List<PoolInfo> Pools { get; set; } = null!;
}