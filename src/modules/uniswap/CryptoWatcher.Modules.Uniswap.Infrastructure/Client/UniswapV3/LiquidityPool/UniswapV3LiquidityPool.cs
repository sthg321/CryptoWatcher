using CryptoWatcher.Modules.Uniswap.Infrastructure.Extensions;
using Nethereum.Contracts;
using Nethereum.Contracts.QueryHandlers.MultiCall;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3;
using UniswapClient.Models;
using UniswapClient.UniswapV3.LiquidityPool.Contracts;

namespace UniswapClient.UniswapV3.LiquidityPool;

public interface IUniswapV3LiquidityPool
{
    Task<LiquidityPoolInfo> GetPoolInfoAsync(IWeb3 web3, string poolAddress,  string multiCallAddress,
        int tickLower, int tickUpper);
}

public class UniswapV3LiquidityPool : IUniswapV3LiquidityPool
{
    public async Task<LiquidityPoolInfo> GetPoolInfoAsync(IWeb3 web3, string poolAddress, string multiCallAddress,
        int tickLower, int tickUpper)
    {
        // 1. Подготавливаем все вызовы
        var calls = new List<Call>
        {
            new()
            {
                Target = poolAddress,
                CallData = new Slot0Function().GetCallData()
            },
            new()
            {
                Target = poolAddress,
                CallData = new FeeGrowthGlobal0X128Function().GetCallData()
            },
            new()
            {
                Target = poolAddress,
                CallData = new FeeGrowthGlobal1X128Function().GetCallData()
            },
            new()
            {
                Target = poolAddress,
                CallData = new TicksFunction { Tick = tickLower }.GetCallData()
            },
            new()
            {
                Target = poolAddress,
                CallData = new TicksFunction { Tick = tickUpper }.GetCallData()
            }
        };

        var result = await web3.MultiCallAsync(calls, multiCallAddress);

        var slot0 = new Slot0OutputDto().DecodeOutput(result[0].ToHex());
        var feeGrowthGlobal0X128 = new FeeGrowthGlobalOutputDTO().DecodeOutput(result[1].ToHex());
        var feeGrowthGlobal1X128 = new FeeGrowthGlobalOutputDTO().DecodeOutput(result[2].ToHex());
        var tickLowerData = new TickInfo().DecodeOutput(result[3].ToHex());
        var tickUpperData = new TickInfo().DecodeOutput(result[4].ToHex());

        return new LiquidityPoolInfo
        {
            Tick = slot0.Tick,
            SqrtPriceX96 = slot0.SqrtPriceX96,
            FeeGrowthGlobal0X128 = feeGrowthGlobal0X128.Value,
            FeeGrowthGlobal1X128 = feeGrowthGlobal1X128.Value,
            LowerTick = new PoolTickInfo
            {
                FeeGrowthOutside0X128 = tickLowerData.FeeGrowthOutside0X128,
                FeeGrowthOutside1X128 = tickLowerData.FeeGrowthOutside1X128
            },
            UpperTick = new PoolTickInfo
            {
                FeeGrowthOutside0X128 = tickUpperData.FeeGrowthOutside0X128,
                FeeGrowthOutside1X128 = tickUpperData.FeeGrowthOutside1X128
            }
        };
    }
}