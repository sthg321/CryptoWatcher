using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV3;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV4;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Mappers;
using CryptoWatcher.Modules.Uniswap.Models;
using CryptoWatcher.Shared.Entities;
using Nethereum.Web3;
using UniswapClient.Models;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services;

/// <summary>
/// <see cref="IUniswapProvider"/> 
/// </summary>
internal class UniswapProvider : IUniswapProvider
{
    private readonly UniswapV3Client _uniswapV3Client;
    private readonly UniswapV4Client _uniswapV4Client;
    private readonly IWeb3Factory _web3Factory;

    public UniswapProvider(UniswapV3Client uniswapV3Client, UniswapV4Client uniswapV4Client, IWeb3Factory web3Factory)
    {
        _uniswapV3Client = uniswapV3Client;
        _uniswapV4Client = uniswapV4Client;
        _web3Factory = web3Factory;
    }

    public async Task<List<IUniswapPosition>> GetPositionsAsync(UniswapChainConfiguration chainConfiguration,
        Wallet wallet)
    {
        return chainConfiguration.ProtocolVersion switch
        {
            UniswapProtocolVersion.V3 => await _uniswapV3Client.PositionFetcher.GetPositionsDataAsync(
                chainConfiguration, wallet.Address),
            UniswapProtocolVersion.V4 => await _uniswapV4Client.PositionFetcher.GetPositionsDataAsync(
                chainConfiguration, wallet.Address),
            _ => throw new ArgumentOutOfRangeException(nameof(chainConfiguration))
        };
    }

    public async Task<LiquidityPool> GetPoolAsync(UniswapChainConfiguration chainConfiguration,
        IUniswapPosition position)
    {
        var pool = position.ProtocolVersion switch
        {
            3 => await GetV3PoolAsync(chainConfiguration, position),
            4 => await GetV4PoolAsync(chainConfiguration, position),
            _ => throw new ArgumentOutOfRangeException(nameof(position), position.ProtocolVersion,
                "Only v3 and v4 protocol supported")
        };

        return pool.MapToLiquidityPool();
    }

    private async Task<LiquidityPoolInfo> GetV3PoolAsync(UniswapChainConfiguration chainConfiguration,
        IUniswapPosition position)
    {
        var web3 = _web3Factory.GetWeb3(chainConfiguration);

        var poolAddress = await _uniswapV3Client.PoolFactory.GetPoolAddressAsync(web3,
            chainConfiguration.SmartContractAddresses.PoolFactory, position.Token0, position.Token1);

        var poolInfoV3 = await _uniswapV3Client.LiquidityPool.GetPoolInfoAsync(web3,
            poolAddress,
            chainConfiguration.SmartContractAddresses.MultiCall,
            position.TickLower, position.TickUpper);

        return poolInfoV3;
    }

    private async Task<LiquidityPoolInfo> GetV4PoolAsync(UniswapChainConfiguration chainConfiguration,
        IUniswapPosition position)
    {
        return await _uniswapV4Client.LiquidityPool.GetPoolAsync(_web3Factory.GetWeb3(chainConfiguration),
            (position as UniswapV4PositionInfo)!);
    }
}