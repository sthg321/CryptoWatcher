using AaveClient.Pool.Contracts;
using Nethereum.Web3;

namespace AaveClient.Pool;

public interface IPoolFetcher
{
    Task<ReserveDataOutput> GetReserveData(string blockchainUrl, string poolAddress, string assetAddress);
}

internal class PoolFetcher : IPoolFetcher
{
    public async Task<ReserveDataOutput> GetReserveData(string blockchainUrl, string poolAddress, string assetAddress)
    {
        var web3 = new Web3(blockchainUrl);
        var contract = web3.Eth.GetContract(PoolAbi.Abi, poolAddress);

        var function = contract.GetFunction("getReserveData");

        var result = await function.CallDeserializingToObjectAsync<ReserveDataOutput>(assetAddress);

        return result;
    }
}