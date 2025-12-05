// CryptoWatcher.Modules.Uniswap.Tests/DataSets/CryptoFaker.cs

using Bogus;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Tests.DataSets;

namespace CryptoWatcher.Modules.Uniswap.Tests.Fakers;

public sealed class UniswapLiquidityPositionFaker : Faker<UniswapLiquidityPosition>
{
    private const int MinTick = -887272;
    private const int MaxTick = 887272;

    public UniswapLiquidityPositionFaker(UniswapChainConfiguration chainConfiguration)
    {
        CustomInstantiator(f =>
        {
            var token0 = f.Crypto().TokenInfo();
            var token1 = f.Crypto().TokenInfoOtherThan(token0);

            var position = new UniswapLiquidityPosition(
                positionId: f.Random.ULong(1),
                tickLower: f.Random.Long(MinTick, 0),
                tickUpper: f.Random.Long(1, MaxTick),
                token0: token0,
                token1: token1,
                walletAddress: f.Crypto().EvmAddress(),
                chain: chainConfiguration,
                f.Date.FutureDateOnly());
            
            return position;
        });
    }
}