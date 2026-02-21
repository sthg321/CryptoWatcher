using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace CryptoWatcher.Modules.Aave.Infrastructure.Client.UiPoolDataProvider.Contracts.UserReserve;

public class UserReserveData
{
    [Parameter("address", "underlyingAsset", 1)]
    public string UnderlyingAsset { get; set; } = null!;

    [Parameter("uint256", "scaledATokenBalance", 2)]
    public BigInteger ScaledATokenBalance { get; set; }

    [Parameter("bool", "usageAsCollateralEnabledOnUser", 3)]
    public bool UsageAsCollateralEnabled { get; set; }

    [Parameter("uint256", "stableBorrowRate", 4)]
    public BigInteger StableBorrowRate { get; set; }

    [Parameter("uint256", "scaledVariableDebt", 5)]
    public BigInteger ScaledVariableDebt { get; set; }

    [Parameter("uint256", "principalStableDebt", 6)]
    public BigInteger PrincipalStableDebt { get; set; }

    [Parameter("uint256", "stableBorrowLastUpdateTimestamp", 7)]
    public BigInteger StableBorrowLastUpdateTimestamp { get; set; }
}