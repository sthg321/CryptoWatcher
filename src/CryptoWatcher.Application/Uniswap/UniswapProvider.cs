using CryptoWatcher.Entities;
using CryptoWatcher.Models;
using Nethereum.Web3;
using UniswapClient.Models;
using UniswapClient.UniswapV3;
using UniswapClient.UniswapV4;

namespace CryptoWatcher.Application.Uniswap;

public class UniswapProvider
{
    private readonly UniswapV3Client _uniswapV3Client;
    private readonly UniswapV4Client _uniswapV4Client;

    public UniswapProvider(UniswapV3Client uniswapV3Client, UniswapV4Client uniswapV4Client)
    {
        _uniswapV3Client = uniswapV3Client;
        _uniswapV4Client = uniswapV4Client;
    }

    public async Task<List<IUniswapPosition>> GetPositionsAsync(Network network, Wallet wallet)
    {
        return network.Name switch
        {
            "ZkSync" => await _uniswapV3Client.PositionFetcher.GetPositionsDataAsync(new Web3(network.RpcUrl),
                new NetworkInfo
                {
                    NetworkUrl = network.RpcUrl,
                    MultiCallAddress = network.MultiCallAddress,
                    NftManagerAddress = network.NftManagerAddress
                }, wallet.Address),

            "Unichain" => await _uniswapV4Client.PositionFetcher.GetPositionsDataAsync(new Web3(network.RpcUrl),
                new NetworkInfo
                {
                    NetworkUrl = network.RpcUrl,
                    MultiCallAddress = network.MultiCallAddress,
                    NftManagerAddress = network.NftManagerAddress
                }, wallet.Address),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<LiquidityPool> GetPoolAsync(IWeb3 web3, Network network,
        IUniswapPosition position)
    {
        switch (position.ProtocolVersion)
        {
            case 3:
            {
                var poolAddress = await _uniswapV3Client.PoolFactory.GetPoolAddressAsync(new Web3(network.RpcUrl),
                    network.PoolFactoryAddress, position.Token0, position.Token1);

                var poolInfoV3 = await _uniswapV3Client.LiquidityPool.GetPoolInfoAsync(new Web3(network.RpcUrl),
                    poolAddress,
                    network.MultiCallAddress,
                    position.TickLower, position.TickUpper);
            
                return Map(poolInfoV3);
            }
            case 4:
            {
                var poolInfoV4 = await _uniswapV4Client.LiquidityPool.GetPoolAsync(new Web3(network.RpcUrl),
                    (position as UniswapV4PositionInfo)!);

                return Map(poolInfoV4);
            }
            default:
                throw new InvalidOperationException($"Protocol version {position.ProtocolVersion} not implemented");
        }
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