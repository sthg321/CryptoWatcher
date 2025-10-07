using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace UniswapClient.UniswapV3.LiquidityPool.Contracts;

[FunctionOutput]
internal class TickInfo : IFunctionOutputDTO
{
    [Parameter("uint128", "liquidityGross", 1)]
    public BigInteger LiquidityGross { get; set; }

    [Parameter("int128", "liquidityNet", 2)]
    public BigInteger LiquidityNet { get; set; }

    [Parameter("int128", "stakedLiquidityNet", 3)]
    public BigInteger StakedLiquidityNet { get; set; }

    [Parameter("uint256", "feeGrowthOutside0X128", 4)]
    public BigInteger FeeGrowthOutside0X128 { get; set; }

    [Parameter("uint256", "feeGrowthOutside1X128", 5)]
    public BigInteger FeeGrowthOutside1X128 { get; set; }

    [Parameter("uint256", "rewardGrowthOutsideX128", 6)]
    public BigInteger RewardGrowthOutsideX128 { get; set; }

    [Parameter("int56", "tickCumulativeOutside", 7)]
    public BigInteger TickCumulativeOutside { get; set; }

    [Parameter("uint160", "secondsPerLiquidityOutsideX128", 8)]
    public BigInteger SecondsPerLiquidityOutsideX128 { get; set; }

    [Parameter("uint32", "secondsOutside", 9)]
    public uint SecondsOutside { get; set; }

    [Parameter("bool", "initialized", 10)] public bool Initialized { get; set; }
}