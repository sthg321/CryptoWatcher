using System.Numerics;
using CryptoWatcher.Modules.Aave.Application.Models;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Aave.Application.Services;

public static class AaveLendingPositionFactory
{
    public static AaveLendingPosition CreateSupply(
        UserReserve userReserve,
        AggregatedMarketReserveData reserve,
        decimal tokenPriceInUsd,
        byte tokenDecimals)
    {
        return Build(
            userReserve.UnderlyingAsset,
            userReserve.ScaledATokenBalance,
            reserve.LiquidityIndex,
            tokenDecimals,
            tokenPriceInUsd,
            AavePositionType.Supplied,
            liquidationLtv: AaveMath.NormalizeLtv(reserve.LiquidationLtv),
            isCollateral: userReserve.IsCollateral
        );
    }

    public static AaveLendingPosition CreateBorrow(
        UserReserve userReserve,
        AggregatedMarketReserveData reserve,
        decimal tokenPriceInUsd,
        byte tokenDecimals)
    {
        return Build(
            userReserve.UnderlyingAsset,
            userReserve.ScaledVariableDebt,
            reserve.VariableBorrowIndex,
            tokenDecimals,
            tokenPriceInUsd,
            AavePositionType.Borrowed
        );
    }

    // ===== CORE =====

    private static AaveLendingPosition Build(
        string underlyingAsset,
        BigInteger scaledBalance,
        BigInteger index,
        byte tokenDecimals,
        decimal tokenPriceInUsd,
        AavePositionType type,
        decimal? liquidationLtv = null,
        bool isCollateral = false)
    {
        var principal = AaveMath.ToTokenDecimal(scaledBalance, tokenDecimals);

        var accruedRaw = AaveMath.CalculateAccruedRaw(scaledBalance, index);

        var actual = AaveMath.ToTokenDecimal(accruedRaw, tokenDecimals);

        return new AaveLendingPosition
        {
            TokenAddress = EvmAddress.Create(underlyingAsset),
            PrincipalAmount = principal,
            Amount = actual,
            AmountUsd = AaveMath.CalculateUsd(actual, tokenPriceInUsd),
            PositionType = type,
            LiquidationLtv = liquidationLtv,
            IsCollateral = isCollateral
        };
    }
}