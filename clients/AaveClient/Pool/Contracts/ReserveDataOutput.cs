using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace AaveClient.Pool.Contracts;

[FunctionOutput]
public class ReserveDataOutput
{
    [Parameter("tuple", "configuration", 1)]
    public ReserveConfigurationMap Configuration { get; set; }

    [Parameter("uint128", "liquidityIndex", 2)]
    public BigInteger LiquidityIndex { get; set; }

    [Parameter("uint128", "currentLiquidityRate", 3)]
    public BigInteger CurrentLiquidityRate { get; set; }

    [Parameter("uint128", "variableBorrowIndex", 4)]
    public BigInteger VariableBorrowIndex { get; set; }

    [Parameter("uint128", "currentVariableBorrowRate", 5)]
    public BigInteger CurrentVariableBorrowRate { get; set; }

    [Parameter("uint128", "currentStableBorrowRate", 6)]
    public BigInteger CurrentStableBorrowRate { get; set; }

    [Parameter("uint40", "lastUpdateTimestamp", 7)]
    public long LastUpdateTimestamp { get; set; }

    [Parameter("uint16", "id", 8)]
    public ushort Id { get; set; }

    [Parameter("address", "aTokenAddress", 9)]
    public string ATokenAddress { get; set; } = null!;

    [Parameter("address", "stableDebtTokenAddress", 10)]
    public string StableDebtTokenAddress { get; set; } = null!;

    [Parameter("address", "variableDebtTokenAddress", 11)]
    public string VariableDebtTokenAddress { get; set; } = null!;

    [Parameter("address", "interestRateStrategyAddress", 12)]
    public string InterestRateStrategyAddress { get; set; } = null!;

    [Parameter("uint128", "accruedToTreasury", 13)]
    public BigInteger AccruedToTreasury { get; set; }

    [Parameter("uint128", "unbacked", 14)]
    public BigInteger Unbacked { get; set; }

    [Parameter("uint128", "isolationModeTotalDebt", 15)]
    public BigInteger IsolationModeTotalDebt { get; set; }
}

[FunctionOutput]
public class ReserveConfigurationMap
{
    [Parameter("uint256", "data", 1)]
    public BigInteger Data { get; set; }
}