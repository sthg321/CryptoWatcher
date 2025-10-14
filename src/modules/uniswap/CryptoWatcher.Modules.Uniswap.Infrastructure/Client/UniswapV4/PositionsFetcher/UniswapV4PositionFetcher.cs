using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV4.PositionsFetcher.Contracts;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV4.StateView;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Services;
using Nethereum.Web3;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV4.PositionsFetcher;

internal interface IUniswapV4PositionFetcher
{
    Task<List<IUniswapPosition>> GetPositionsDataAsync(UniswapChainConfiguration chain,
        string walletAddress);
}

internal class UniswapV4PositionFetcher : IUniswapV4PositionFetcher
{
    private readonly UniswapAppApiClient.UniswapAppApiClient _apiClient;
    private readonly IUniswapV4StateView _stateView;
    private readonly IWeb3Factory _web3Factory;

    public UniswapV4PositionFetcher(UniswapAppApiClient.UniswapAppApiClient apiClient, IUniswapV4StateView stateView,
        IWeb3Factory web3Factory)
    {
        _apiClient = apiClient;
        _stateView = stateView;
        _web3Factory = web3Factory;
    }

    public async Task<List<IUniswapPosition>> GetPositionsDataAsync(UniswapChainConfiguration chain,
        string walletAddress)
    {
        var web3 = _web3Factory.GetWeb3(chain);
        var tokenIds = await _apiClient.GetPoolPositionTokenIdsAsync(walletAddress);

        return await GetPositionsDataAsync(web3, chain, tokenIds);
    }

    private async Task<List<IUniswapPosition>> GetPositionsDataAsync(IWeb3 web3,
        UniswapChainConfiguration chain,
        IReadOnlyCollection<ulong> tokenIds)
    {
        var result = new List<IUniswapPosition>();
        foreach (var tokenId in tokenIds)
        {
            var contract =
                web3.Eth.GetContract(UniswapV4PositionFetcherAbi.Abi, chain.SmartContractAddresses.NftManager);

            var packedData = await contract.GetFunction("getPoolAndPositionInfo")
                .CallDeserializingToObjectAsync<GetPoolAndPositionInfoOutputDTO>(tokenId);

            var positionInfo = PositionInfoParser.FromUInt256(packedData.PositionInfo);

            var poolKey = new UniswapV4PoolKey
            {
                Currency0 = packedData.PoolKey.Currency0,
                Currency1 = packedData.PoolKey.Currency1,
                Fee = packedData.PoolKey.Fee,
                TickSpacing = packedData.PoolKey.TickSpacing,
                Hooks = packedData.PoolKey.Hooks,
            };

            var feeGrowth = await _stateView.GetPositionInfoAsync(web3, poolKey, positionInfo.TickLower,
                positionInfo.TickUpper, tokenId);

            result.Add(new UniswapV4PositionInfo
            {
                PoolKey = poolKey,
                PositionId = tokenId,
                TickLower = positionInfo.TickLower,
                TickUpper = positionInfo.TickUpper,
                Token0 = packedData.PoolKey.Currency0,
                Token1 = packedData.PoolKey.Currency1,
                Liquidity = feeGrowth.Liquidity,
                FeeGrowthInside0LastX128 = feeGrowth.FeeGrowthInside0LastX128,
                FeeGrowthInside1LastX128 = feeGrowth.FeeGrowthInside1LastX128
            });
        }

        return result;
    }
}