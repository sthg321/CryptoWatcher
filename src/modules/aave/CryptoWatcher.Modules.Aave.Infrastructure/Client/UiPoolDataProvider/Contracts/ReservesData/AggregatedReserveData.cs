using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace CryptoWatcher.Modules.Aave.Infrastructure.Client.UiPoolDataProvider.Contracts.ReservesData;

public class AggregatedReserveData
{
    [Parameter("address", "underlyingAsset", 1)]
    public string UnderlyingAsset { get; set; }

    [Parameter("string", "name", 2)] public string Name { get; set; }

    [Parameter("string", "symbol", 3)] public string Symbol { get; set; }

    [Parameter("uint256", "decimals", 4)] public BigInteger Decimals { get; set; }

    [Parameter("uint256", "baseLTVasCollateral", 5)]
    public BigInteger BaseLTVasCollateral { get; set; }

    [Parameter("uint256", "reserveLiquidationThreshold", 6)]
    public BigInteger ReserveLiquidationThreshold { get; set; }

    [Parameter("uint256", "reserveLiquidationBonus", 7)]
    public BigInteger ReserveLiquidationBonus { get; set; }

    [Parameter("uint256", "reserveFactor", 8)]
    public BigInteger ReserveFactor { get; set; }

    [Parameter("bool", "usageAsCollateralEnabled", 9)]
    public bool UsageAsCollateralEnabled { get; set; }

    [Parameter("bool", "borrowingEnabled", 10)]
    public bool BorrowingEnabled { get; set; }

    [Parameter("bool", "stableBorrowRateEnabled", 11)]
    public bool StableBorrowRateEnabled { get; set; }

    [Parameter("bool", "isActive", 12)] public bool IsActive { get; set; }

    [Parameter("bool", "isFrozen", 13)] public bool IsFrozen { get; set; }

    [Parameter("uint128", "liquidityIndex", 14)]
    public BigInteger LiquidityIndex { get; set; }

    [Parameter("uint128", "variableBorrowIndex", 15)]
    public BigInteger VariableBorrowIndex { get; set; }

    [Parameter("uint128", "liquidityRate", 16)]
    public BigInteger LiquidityRate { get; set; }

    [Parameter("uint128", "variableBorrowRate", 17)]
    public BigInteger VariableBorrowRate { get; set; }

    [Parameter("uint128", "stableBorrowRate", 18)]
    public BigInteger StableBorrowRate { get; set; }

    [Parameter("uint40", "lastUpdateTimestamp", 19)]
    public BigInteger LastUpdateTimestamp { get; set; }

    [Parameter("address", "aTokenAddress", 20)]
    public string ATokenAddress { get; set; }

    [Parameter("address", "stableDebtTokenAddress", 21)]
    public string StableDebtTokenAddress { get; set; }

    [Parameter("address", "variableDebtTokenAddress", 22)]
    public string VariableDebtTokenAddress { get; set; }

    [Parameter("address", "interestRateStrategyAddress", 23)]
    public string InterestRateStrategyAddress { get; set; }

    [Parameter("uint256", "availableLiquidity", 24)]
    public BigInteger AvailableLiquidity { get; set; }

    [Parameter("uint256", "totalPrincipalStableDebt", 25)]
    public BigInteger TotalPrincipalStableDebt { get; set; }

    [Parameter("uint256", "averageStableRate", 26)]
    public BigInteger AverageStableRate { get; set; }

    [Parameter("uint256", "stableDebtLastUpdateTimestamp", 27)]
    public BigInteger StableDebtLastUpdateTimestamp { get; set; }

    [Parameter("uint256", "totalScaledVariableDebt", 28)]
    public BigInteger TotalScaledVariableDebt { get; set; }

    [Parameter("uint256", "priceInMarketReferenceCurrency", 29)]
    public BigInteger PriceInMarketReferenceCurrency { get; set; }

    [Parameter("address", "priceOracle", 30)]
    public string PriceOracle { get; set; }

    [Parameter("uint256", "variableRateSlope1", 31)]
    public BigInteger VariableRateSlope1 { get; set; }

    [Parameter("uint256", "variableRateSlope2", 32)]
    public BigInteger VariableRateSlope2 { get; set; }

    [Parameter("uint256", "stableRateSlope1", 33)]
    public BigInteger StableRateSlope1 { get; set; }

    [Parameter("uint256", "stableRateSlope2", 34)]
    public BigInteger StableRateSlope2 { get; set; }

    [Parameter("uint256", "baseStableBorrowRate", 35)]
    public BigInteger BaseStableBorrowRate { get; set; }

    [Parameter("uint256", "baseVariableBorrowRate", 36)]
    public BigInteger BaseVariableBorrowRate { get; set; }

    [Parameter("uint256", "optimalUsageRatio", 37)]
    public BigInteger OptimalUsageRatio { get; set; }

    [Parameter("bool", "isPaused", 38)] public bool IsPaused { get; set; }

    [Parameter("bool", "isSiloedBorrowing", 39)]
    public bool IsSiloedBorrowing { get; set; }

    [Parameter("uint128", "accruedToTreasury", 40)]
    public BigInteger AccruedToTreasury { get; set; }

    [Parameter("uint128", "unbacked", 41)] public BigInteger Unbacked { get; set; }

    [Parameter("uint128", "isolationModeTotalDebt", 42)]
    public BigInteger IsolationModeTotalDebt { get; set; }

    [Parameter("bool", "flashLoanEnabled", 43)]
    public bool FlashLoanEnabled { get; set; }

    [Parameter("uint256", "debtCeiling", 44)]
    public BigInteger DebtCeiling { get; set; }

    [Parameter("uint256", "debtCeilingDecimals", 45)]
    public BigInteger DebtCeilingDecimals { get; set; }

    [Parameter("uint8", "eModeCategoryId", 46)]
    public BigInteger EModeCategoryId { get; set; }

    [Parameter("uint256", "borrowCap", 47)]
    public BigInteger BorrowCap { get; set; }

    [Parameter("uint256", "supplyCap", 48)]
    public BigInteger SupplyCap { get; set; }

    [Parameter("uint16", "eModeLtv", 49)] public BigInteger EModeLtv { get; set; }

    [Parameter("uint16", "eModeLiquidationThreshold", 50)]
    public BigInteger EModeLiquidationThreshold { get; set; }

    [Parameter("uint16", "eModeLiquidationBonus", 51)]
    public BigInteger EModeLiquidationBonus { get; set; }

    [Parameter("address", "eModePriceSource", 52)]
    public string EModePriceSource { get; set; }

    [Parameter("string", "eModeLabel", 53)]
    public string EModeLabel { get; set; }

    [Parameter("bool", "borrowableInIsolation", 54)]
    public bool BorrowableInIsolation { get; set; }
}