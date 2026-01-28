using System.Net;
using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV4.PositionsFetcher.Contracts;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV4.StateView;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Blockchain;
using Nethereum.Web3;
using Polly;
using Polly.Retry;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV4.PositionsFetcher;

internal interface IUniswapV4PositionFetcher
{
    Task<List<IUniswapPosition>> GetPositionsDataAsync(UniswapChainConfiguration chain,
        IReadOnlyCollection<ulong> tokenIds);
}

internal class UniswapV4PositionFetcher : IUniswapV4PositionFetcher
{
    private readonly IUniswapV4StateView _stateView;
    private readonly IWeb3Factory _web3Factory;

    private static readonly AsyncRetryPolicy TimeoutPolicy =
        Policy.Handle<HttpRequestException>(exception => exception.StatusCode == HttpStatusCode.RequestTimeout)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    public UniswapV4PositionFetcher(IUniswapV4StateView stateView, IWeb3Factory web3Factory)
    {
        _stateView = stateView;
        _web3Factory = web3Factory;
    }

    public async Task<List<IUniswapPosition>> GetPositionsDataAsync(UniswapChainConfiguration chain,
        IReadOnlyCollection<ulong> tokenIds)
    {
        var web3 = _web3Factory.GetWeb3(chain);

        return await GetPositionsDataAsync(web3, chain, tokenIds);
    }

    private async Task<List<IUniswapPosition>> GetPositionsDataAsync(IWeb3 web3,
        UniswapChainConfiguration chain,
        IReadOnlyCollection<ulong> tokenIds)
    {
        var result = new List<IUniswapPosition>(tokenIds.Count);

        foreach (var tokenId in tokenIds)
        {
            var contract =
                web3.Eth.GetContract(UniswapV4PositionFetcherAbi.Abi, chain.SmartContractAddresses.PositionManager);

            var packedData = await TimeoutPolicy.ExecuteAsync(_ => contract.GetFunction("getPoolAndPositionInfo")
                .CallDeserializingToObjectAsync<GetPoolAndPositionInfoOutputDTO>(tokenId), CancellationToken.None);

            var positionInfo = PositionInfoParser.FromUInt256(packedData.PositionInfo);

            var poolKey = new UniswapV4PoolKey
            {
                Currency0 = packedData.PoolKey.Currency0,
                Currency1 = packedData.PoolKey.Currency1,
                Fee = packedData.PoolKey.Fee,
                TickSpacing = packedData.PoolKey.TickSpacing,
                Hooks = packedData.PoolKey.Hooks,
            };

            var feeGrowth = await _stateView.GetPositionInfoAsync(chain, poolKey, positionInfo.TickLower,
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