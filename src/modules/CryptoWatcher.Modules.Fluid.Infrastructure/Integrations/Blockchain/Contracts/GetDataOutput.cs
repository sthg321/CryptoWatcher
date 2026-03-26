using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace CryptoWatcher.Modules.Fluid.Infrastructure.Integrations.Blockchain.Contracts;

[FunctionOutput]
public class GetDataOutput : IFunctionOutputDTO
{
    [Parameter("address", "liquidity_", 1)]
    public string Liquidity { get; set; } = null!;

    [Parameter("address", "lendingFactory_", 2)]
    public string LendingFactory { get; set; } = null!;

    [Parameter("address", "lendingRewardsRateModel_", 3)]
    public string LendingRewardsRateModel { get; set; } = null!;

    [Parameter("address", "permit2_", 4)] public string Permit2 { get; set; } = null!;

    [Parameter("address", "rebalancer_", 5)]
    public string Rebalancer { get; set; } = null!;

    [Parameter("bool", "rewardsActive_", 6)]
    public bool RewardsActive { get; set; }

    [Parameter("uint256", "liquidityBalance_", 7)]
    public BigInteger LiquidityBalance { get; set; }

    [Parameter("uint256", "liquidityExchangePrice_", 8)]
    public BigInteger LiquidityExchangePrice { get; set; }

    [Parameter("uint256", "tokenExchangePrice_", 9)]
    public BigInteger TokenExchangePrice { get; set; }
}