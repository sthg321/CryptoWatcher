using System.Numerics;
using Nethereum.Contracts;
using Nethereum.Contracts.QueryHandlers.MultiCall;
using Nethereum.Contracts.Standards.ERC721.ContractDefinition;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3;
using UniswapClient.Extensions;
using UniswapClient.Models;
using UniswapClient.UniswapV3.PositionsFetcher.Contracts;

namespace UniswapClient.UniswapV3.PositionsFetcher;

public interface IUniswapV3PositionFetcher
{
    Task<List<IUniswapPosition>> GetPositionsDataAsync(IWeb3 web3,
        NetworkInfo network,
        string walletAddress);
}

internal class UniswapV3PositionFetcher : IUniswapV3PositionFetcher
{
    public async Task<List<IUniswapPosition>> GetPositionsDataAsync(IWeb3 web3,
        NetworkInfo network,
        string walletAddress)
    {
        var balance = await web3.Eth.ERC20.GetContractService(network.NftManagerAddress)
            .BalanceOfQueryAsync(walletAddress);

        var tokenIds = await GetTokenIdsAsync(web3, network, walletAddress, balance);

        return await GetPositionsDataAsync(web3, network, tokenIds);
    }

    private static async Task<List<BigInteger>> GetTokenIdsAsync(IWeb3 web3, NetworkInfo network, string walletAddress,
        BigInteger count)
    {
        var calls = Enumerable.Range(0, (int)count).Select(i => new Call
        {
            Target = network.NftManagerAddress, CallData = new TokenOfOwnerByIndexFunction
            {
                Owner = walletAddress,
                Index = i
            }.GetCallData()
        }).ToList();

        return await web3.MultiCallAsync(calls, network.MultiCallAddress,
            bytes =>
            {
                var response = new TokenOfOwnerByIndexOutputDTO().DecodeOutput(bytes.ToHex());
                return response.ReturnValue1;
            });
    }

    private static async Task<List<IUniswapPosition>> GetPositionsDataAsync(IWeb3 web3, NetworkInfo network,
        List<BigInteger> tokenIds)
    {
        var calls = tokenIds.Select(tokenId => new Call
            {
                Target = network.NftManagerAddress, CallData = new PositionsFunction { TokenId = tokenId }.GetCallData()
            })
            .ToList();

        var result = await web3.MultiCallAsync(calls, network.MultiCallAddress,
            bytes => new PositionsOutputDTO().DecodeOutput(bytes.ToHex()));

        return result.Select((output, i) => new UniswapV3PositionInfo
            {
                Token0 = output.Token0,
                Token1 = output.Token1,
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