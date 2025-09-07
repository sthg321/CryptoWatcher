using System.Numerics;
using Nethereum.Web3;

namespace AaveClient.AaveOracle;

public interface IAaveOracleFetcher
{
    Task<BigInteger> GetAssetPriceAsync(string blockchainUrl, string oracleAddress, string assetAddress);
}

internal class AaveOracleFetcher : IAaveOracleFetcher
{
    public async Task<BigInteger> GetAssetPriceAsync(string blockchainUrl, string oracleAddress, string assetAddress)
    {
        var web3 = new Web3(blockchainUrl);
        var contract = web3.Eth.GetContract(AaveOracleFetcherAbi.Abi, oracleAddress);

        var function = contract.GetFunction("getAssetPrice");

        return await function.CallAsync<BigInteger>(assetAddress);
    }
}