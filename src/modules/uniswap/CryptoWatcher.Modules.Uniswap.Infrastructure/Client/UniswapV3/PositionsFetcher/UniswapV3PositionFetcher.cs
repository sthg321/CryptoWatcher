using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV3.PositionsFetcher.Contracts;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Extensions;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Blockchain;
using Nethereum.Contracts;
using Nethereum.Contracts.QueryHandlers.MultiCall;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Client.UniswapV3.PositionsFetcher;

public interface IUniswapV3PositionFetcher
{
    Task<List<IUniswapPosition>> GetPositionsDataAsync(UniswapChainConfiguration chain,
        List<ulong> tokenIds);
}

internal class UniswapV3PositionFetcher : IUniswapV3PositionFetcher
{
    private readonly IWeb3Factory _web3Factory;

    public UniswapV3PositionFetcher(IWeb3Factory web3Factory)
    {
        _web3Factory = web3Factory;
    }

    public async Task<List<IUniswapPosition>> GetPositionsDataAsync(UniswapChainConfiguration chain,
        List<ulong> tokenIds)
    {
        var web3 = _web3Factory.GetWeb3(chain);

        return await GetPositionsDataAsync(web3, chain, tokenIds);
    }

    private static async Task<List<IUniswapPosition>> GetPositionsDataAsync(IWeb3 web3, UniswapChainConfiguration chain,
        List<ulong> tokenIds)
    {
        var calls = tokenIds.Select(tokenId => new Call
            {
                Target = chain.SmartContractAddresses.PositionManager,
                CallData = new PositionsFunction { TokenId = tokenId }.GetCallData()
            })
            .ToList();

        var result = await web3.MultiCallAsync(calls, chain.SmartContractAddresses.MultiCall,
            bytes => new PositionsOutputDTO().DecodeOutput(bytes.ToHex()));

        return result.Select((output, i) => new UniswapV3PositionInfo
            {
                Token0 = output.Token0,
                Token1 = output.Token1,
                Fee = output.Fee,
                TickLower = output.TickLower,
                TickUpper = output.TickUpper,
                Liquidity = output.Liquidity,
                FeeGrowthInside0LastX128 = output.FeeGrowthInside0LastX128,
                FeeGrowthInside1LastX128 = output.FeeGrowthInside1LastX128,
                PositionId = tokenIds[i]
            })
            .Cast<IUniswapPosition>()
            .ToList();
    }
}