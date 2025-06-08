using CryptoWatcher.Core;
using CryptoWatcher.Entities;
using CryptoWatcher.Entities.Uniswap;
using CryptoWatcher.Integrations;
using CryptoWatcher.Models;
using Nethereum.Web3;
using UniswapClient.Models;
using UniswapClient.UniswapV3;
using UniswapClient.UniswapV4;

namespace CryptoWatcher.Host.Integrations;

public class UniswapProvider : IUniswapProvider
{
    private static class UniswapProtocolVersion
    {
        public const int V3 = 3;
        public const int V4 = 4;
    }

    private readonly UniswapV3Client _uniswapV3Client;
    private readonly UniswapV4Client _uniswapV4Client;
    private readonly IUniswapMath _uniswapMath;

    public UniswapProvider(UniswapV3Client uniswapV3Client, UniswapV4Client uniswapV4Client, IUniswapMath uniswapMath)
    {
        _uniswapV3Client = uniswapV3Client;
        _uniswapV4Client = uniswapV4Client;
        _uniswapMath = uniswapMath;
    }

    public async Task<List<IUniswapPosition>> GetPositionsAsync(UniswapNetwork uniswapNetwork, Wallet wallet)
    {
        var result = new List<IUniswapPosition>();
        var networkInfo = new NetworkInfo
        {
            NetworkUrl = uniswapNetwork.RpcUrl,
            MultiCallAddress = uniswapNetwork.MultiCallAddress,
            NftManagerAddress = uniswapNetwork.NftManagerAddress
        };

        if ((uniswapNetwork.SupportedProtocolVersions & Entities.Uniswap.UniswapProtocolVersion.V3) ==
            Entities.Uniswap.UniswapProtocolVersion.V3)
        {
            result.AddRange(await _uniswapV3Client.PositionFetcher.GetPositionsDataAsync(
                new Web3(uniswapNetwork.RpcUrl),
                networkInfo, wallet.Address));
        }

        if ((uniswapNetwork.SupportedProtocolVersions & Entities.Uniswap.UniswapProtocolVersion.V4) ==
            Entities.Uniswap.UniswapProtocolVersion.V4)
        {
            result.AddRange(await _uniswapV4Client.PositionFetcher.GetPositionsDataAsync(
                new Web3(uniswapNetwork.RpcUrl),
                networkInfo, wallet.Address));
        }

        return result;
    }

    public async Task<LiquidityPool> GetPoolAsync(UniswapNetwork uniswapNetwork, IUniswapPosition position)
    {
        var pool = position.ProtocolVersion switch
        {
            UniswapProtocolVersion.V3 => await GetV3PoolAsync(uniswapNetwork, position),
            UniswapProtocolVersion.V4 => await GetV4PoolAsync(uniswapNetwork, position),
            _ => throw new ArgumentOutOfRangeException(nameof(position), position.ProtocolVersion,
                "Only v3 and v4 protocol supported ")
        };

        return Map(pool);
    }

    public PositionInPool GetPoolPositionAsync(LiquidityPool pool, IUniswapPosition uniswapPosition)
    {
        return _uniswapMath.CalculatePosition(pool, uniswapPosition);
    }

    public TokenPair GetPositionFee(LiquidityPool pool, IUniswapPosition uniswapPosition)
    {
        return _uniswapMath.CalculateClaimableFee(pool, uniswapPosition);
    }

    private async Task<LiquidityPoolInfo> GetV3PoolAsync(UniswapNetwork uniswapNetwork, IUniswapPosition position)
    {
        var poolAddress = await _uniswapV3Client.PoolFactory.GetPoolAddressAsync(new Web3(uniswapNetwork.RpcUrl),
            uniswapNetwork.PoolFactoryAddress, position.Token0, position.Token1);

        var poolInfoV3 = await _uniswapV3Client.LiquidityPool.GetPoolInfoAsync(new Web3(uniswapNetwork.RpcUrl),
            poolAddress,
            uniswapNetwork.MultiCallAddress,
            position.TickLower, position.TickUpper);

        return poolInfoV3;
    }

    private async Task<LiquidityPoolInfo> GetV4PoolAsync(UniswapNetwork uniswapNetwork, IUniswapPosition position)
    {
        return await _uniswapV4Client.LiquidityPool.GetPoolAsync(new Web3(uniswapNetwork.RpcUrl),
            (position as UniswapV4PositionInfo)!);
    }

    private static LiquidityPool Map(LiquidityPoolInfo poolInfo)
    {
        return new LiquidityPool
        {
            Tick = poolInfo.Tick,
            LowerTick = new LiquidityPoolTick
            {
                FeeGrowthOutside0X128 = poolInfo.LowerTick.FeeGrowthOutside0X128,
                FeeGrowthOutside1X128 = poolInfo.LowerTick.FeeGrowthOutside1X128,
            },
            UpperTick = new LiquidityPoolTick
            {
                FeeGrowthOutside0X128 = poolInfo.UpperTick.FeeGrowthOutside0X128,
                FeeGrowthOutside1X128 = poolInfo.UpperTick.FeeGrowthOutside1X128,
            },
            SqrtPriceX96 = poolInfo.SqrtPriceX96,
            FeeGrowthGlobal0X128 = poolInfo.FeeGrowthGlobal0X128,
            FeeGrowthGlobal1X128 = poolInfo.FeeGrowthGlobal1X128,
        };
    }
}