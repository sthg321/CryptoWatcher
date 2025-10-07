using Nethereum.Web3;
using UniswapClient.Models;
using UniswapClient.UniswapV4.StateView;

namespace UniswapClient.UniswapV4.LiquidityPool;

public interface IUniswapV4LiquidityPool
{
    Task<LiquidityPoolInfo> GetPoolAsync(IWeb3 web3,  UniswapV4PositionInfo position);
}

internal class UniswapV4LiquidityPool : IUniswapV4LiquidityPool
{
    private readonly IUniswapV4StateView _stateView;

    public UniswapV4LiquidityPool(IUniswapV4StateView stateView)
    {
        _stateView = stateView;
    }

    public async Task<LiquidityPoolInfo> GetPoolAsync(IWeb3 web3,  UniswapV4PositionInfo position)
    {
        var sot0 = await _stateView.GetSlot0Async(web3, position.PoolKey);

        var tickLower = await _stateView.GetTickInfoAsync(web3, position.PoolKey, position.TickLower);

        var tickUpper = await _stateView.GetTickInfoAsync(web3, position.PoolKey, position.TickUpper);

        var feeGlobal = await _stateView.GetFeeGrowGlobalAsync(web3, position.PoolKey);

        return new LiquidityPoolInfo
        {
            SqrtPriceX96 = sot0.SqrtPriceX96,
            Tick = sot0.Tick,
            FeeGrowthGlobal0X128 = feeGlobal.FeeGrowthGlobal0,
            FeeGrowthGlobal1X128 = feeGlobal.FeeGrowthGlobal1,
            LowerTick = new PoolTickInfo
            {
                FeeGrowthOutside0X128 = tickLower.FeeGrowthOutside0X128,
                FeeGrowthOutside1X128 = tickLower.FeeGrowthOutside1X128
            },
            UpperTick = new PoolTickInfo
            {
                FeeGrowthOutside0X128 = tickUpper.FeeGrowthOutside0X128,
                FeeGrowthOutside1X128 = tickUpper.FeeGrowthOutside1X128
            }
        };
    }
}