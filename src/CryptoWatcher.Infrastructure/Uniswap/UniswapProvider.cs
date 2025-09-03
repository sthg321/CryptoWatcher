using CryptoWatcher.Infrastructure.Uniswap.Mappers;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.UniswapModule;
using CryptoWatcher.UniswapModule.Abstractions;
using CryptoWatcher.UniswapModule.Entities;
using CryptoWatcher.UniswapModule.Models;
using Nethereum.Web3;
using UniswapClient.Models;
using UniswapClient.UniswapV3;
using UniswapClient.UniswapV4;

namespace CryptoWatcher.Infrastructure.Uniswap;

/// <summary>
/// <see cref="IUniswapProvider"/> 
/// </summary>
internal class UniswapProvider : IUniswapProvider
{
    private readonly UniswapV3Client _uniswapV3Client;
    private readonly UniswapV4Client _uniswapV4Client;

    public UniswapProvider(UniswapV3Client uniswapV3Client, UniswapV4Client uniswapV4Client)
    {
        _uniswapV3Client = uniswapV3Client;
        _uniswapV4Client = uniswapV4Client;
    }

    public async Task<List<IUniswapPosition>> GetPositionsAsync(UniswapNetwork uniswapNetwork, Wallet wallet)
    {
        var networkInfo = new NetworkInfo
        {
            NetworkUrl = uniswapNetwork.RpcUrl,
            MultiCallAddress = uniswapNetwork.MultiCallAddress,
            NftManagerAddress = uniswapNetwork.NftManagerAddress
        };

        return uniswapNetwork.ProtocolVersion switch
        {
            UniswapProtocolVersion.V3 => await _uniswapV3Client.PositionFetcher.GetPositionsDataAsync(
                networkInfo, wallet.Address),
            UniswapProtocolVersion.V4 => await _uniswapV4Client.PositionFetcher.GetPositionsDataAsync(
                networkInfo, wallet.Address),
            _ => throw new ArgumentOutOfRangeException(nameof(uniswapNetwork))
        };
    }

    public async Task<LiquidityPool> GetPoolAsync(UniswapNetwork uniswapNetwork, IUniswapPosition position)
    {
        var pool = position.ProtocolVersion switch
        {
            3 => await GetV3PoolAsync(uniswapNetwork, position),
            4 => await GetV4PoolAsync(uniswapNetwork, position),
            _ => throw new ArgumentOutOfRangeException(nameof(position), position.ProtocolVersion,
                "Only v3 and v4 protocol supported")
        };

        return pool.MapToLiquidityPool();
    }

    private async Task<LiquidityPoolInfo> GetV3PoolAsync(UniswapNetwork uniswapNetwork, IUniswapPosition position)
    {
        var web3 = new Web3(uniswapNetwork.RpcUrl);
        
        var poolAddress = await _uniswapV3Client.PoolFactory.GetPoolAddressAsync(web3,
            uniswapNetwork.PoolFactoryAddress, position.Token0, position.Token1);

        var poolInfoV3 = await _uniswapV3Client.LiquidityPool.GetPoolInfoAsync(web3,
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
}