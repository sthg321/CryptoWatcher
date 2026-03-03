using System.Numerics;
using CryptoWatcher.Extensions;
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
            reserve.Symbol,
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
            AavePositionType.Borrowed,
            reserve.Symbol
        );
    }

    private static AaveLendingPosition Build(
        string underlyingAsset,
        BigInteger scaledBalance,
        BigInteger index,
        byte tokenDecimals,
        decimal tokenPriceInUsd,
        AavePositionType type,
        string symbol,
        decimal? liquidationLtv = null,
        bool isCollateral = false)
    {
        var principal = scaledBalance.ToDecimal(tokenDecimals);

        var accruedRaw = AaveMath.CalculateAccruedRaw(scaledBalance, index);

        return new AaveLendingPosition
        {
            TokenAddress = EvmAddress.Create(underlyingAsset),
            PrincipalAmount = principal,
            Amount = accruedRaw.ToDecimal(tokenDecimals),
            TokenPriceInUsd = tokenPriceInUsd,
            PositionType = type,
            LiquidationLtv = liquidationLtv,
            IsCollateral = isCollateral,
            TokenSymbol = symbol
        };
    }
}