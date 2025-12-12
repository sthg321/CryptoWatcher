using System.Numerics;
using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Helpers;
using CryptoWatcher.Modules.Uniswap.Models;
using CryptoWatcher.Shared.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure;

// ReSharper disable InconsistentNaming
internal class UniswapMath : IUniswapMath
{
    private static readonly BigInteger Q96 = BigInteger.Pow(2, 96);

    public PositionInPool CalculatePosition(LiquidityPool pool, IUniswapPosition position)
    {
        var sqrtPriceX96 = pool.SqrtPriceX96;

        var sqrtPriceAx96 = TickMath.GetSqrtRatioAtTick(position.TickLower);
        var sqrtPriceBx96 = TickMath.GetSqrtRatioAtTick(position.TickUpper);

        var (amount0, amount1) = CalculateTokenAmounts(
            sqrtPriceX96,
            sqrtPriceAx96,
            sqrtPriceBx96,
            position.Liquidity);

        return new PositionInPool
        {
            PositionId = (ulong)position.PositionId,
            TokenInfoPair = new TokenPair
            {
                Token0 = new Token { Address = position.Token0, Balance = amount0 },
                Token1 = new Token { Address = position.Token1, Balance = amount1 }
            },
            IsInRange = pool.Tick >= position.TickLower && pool.Tick < position.TickUpper
        };
    }

    public TokenPair CalculateClaimableFee(LiquidityPool pool, IUniswapPosition position)
    {
        var feeGrowthInside0 = GetFeeGrowthInside(
            position.TickLower, position.TickUpper, pool.Tick,
            pool.FeeGrowthGlobal0X128,
            pool.LowerTick.FeeGrowthOutside0X128,
            pool.UpperTick.FeeGrowthOutside0X128
        );

        var feeGrowthInside1 = GetFeeGrowthInside(
            position.TickLower, position.TickUpper, pool.Tick,
            pool.FeeGrowthGlobal1X128,
            pool.LowerTick.FeeGrowthOutside1X128,
            pool.UpperTick.FeeGrowthOutside1X128
        );
    
        var diff0 = CalculateFeeGrowthDiff(feeGrowthInside0, position.FeeGrowthInside0LastX128);
        var diff1 = CalculateFeeGrowthDiff(feeGrowthInside1, position.FeeGrowthInside1LastX128);
    
        var earned0 = position.Liquidity * diff0 / BigInteger.Pow(2, 128);
        var earned1 = position.Liquidity * diff1 / BigInteger.Pow(2, 128);
    
        return new TokenPair
        {
            Token0 = new Token { Address = position.Token0, Balance = earned0 },
            Token1 = new Token { Address = position.Token1, Balance = earned1 }
        };
    }

    private BigInteger CalculateFeeGrowthDiff(BigInteger current, BigInteger last)
    {
        if (current >= last)
        {
            return current - last;
        }

        // (type(uint256).max - last) + current + 1
        var maxUint256 = BigInteger.Pow(2, 256) - 1;
        return maxUint256 - last + current + 1;
    }
    
    private static (BigInteger amount0, BigInteger amount1) CalculateTokenAmounts(
        BigInteger sqrtRatioX96,
        BigInteger sqrtRatioAX96,
        BigInteger sqrtRatioBX96,
        BigInteger liquidity)
    {
        (sqrtRatioAX96, sqrtRatioBX96) = NormalizeSqrtRatioBounds(sqrtRatioAX96, sqrtRatioBX96);

        if (sqrtRatioX96 < sqrtRatioAX96)
        {
            var amount0 = GetAmount0ForLiquidity(sqrtRatioAX96, sqrtRatioBX96, liquidity);
            return (amount0, 0);
        }

        if (sqrtRatioX96 < sqrtRatioBX96)
        {
            return
            (
                GetAmount0ForLiquidity(sqrtRatioX96, sqrtRatioBX96, liquidity),
                GetAmount1ForLiquidity(sqrtRatioAX96, sqrtRatioX96, liquidity)
            );
        }

        var amount1 = GetAmount1ForLiquidity(sqrtRatioAX96, sqrtRatioBX96, liquidity);
        return (0, amount1);
    }

    private static BigInteger GetFeeGrowthInside(
        int tickLower,
        int tickUpper,
        int currentTick,
        BigInteger feeGrowthGlobal,
        BigInteger feeGrowthOutsideLower,
        BigInteger feeGrowthOutsideUpper)
    {
        BigInteger feeGrowthBelow;

        if (currentTick >= tickLower)
        {
            feeGrowthBelow = feeGrowthOutsideLower;
        }
        else
        {
            feeGrowthBelow = feeGrowthGlobal - feeGrowthOutsideLower;
        }

        var feeGrowthAbove = currentTick < tickUpper ? feeGrowthOutsideUpper : feeGrowthGlobal - feeGrowthOutsideUpper;

        return feeGrowthGlobal - feeGrowthBelow - feeGrowthAbove;
    }
    
    private static BigInteger GetAmount0ForLiquidity(BigInteger sqrtRatioAx96, BigInteger sqrtRatioBx96,
        BigInteger liquidity)
    {
        var numerator = BigInteger.Multiply(liquidity << 96, sqrtRatioBx96 - sqrtRatioAx96);
        var denominator = BigInteger.Multiply(sqrtRatioBx96, sqrtRatioAx96);
        return numerator / denominator;
    }

    private static BigInteger GetAmount1ForLiquidity(BigInteger sqrtRatioAX96, BigInteger sqrtRatioBX96,
        BigInteger liquidity)
    {
        return BigInteger.Multiply(liquidity, sqrtRatioBX96 - sqrtRatioAX96) / Q96;
    }

    private static (BigInteger, BigInteger) NormalizeSqrtRatioBounds(BigInteger lower, BigInteger upper)
    {
        return lower > upper ? (upper, lower) : (lower, upper);
    }
}