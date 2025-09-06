using AaveClient.UiPoolDataProvider.Contracts;
using Nethereum.Web3;

namespace AaveClient.UiPoolDataProvider;

public interface IUiPoolDataProviderFetcher
{
    Task<List<UserReserveData>> GetUserReservesDataAsync(string mainnetAddress, NetworkRegistry.NetworkInfo networkInfo,
        string userAddress);
}

public class UiPoolDataProviderFetcher : IUiPoolDataProviderFetcher
{
    public async Task<List<UserReserveData>> GetUserReservesDataAsync(
        string mainnetAddress,
        NetworkRegistry.NetworkInfo networkInfo,
        string userAddress)
    {
        var web3 = new Web3(mainnetAddress);

        var contract = web3.Eth.GetContract(UiPoolDataProviderFetcherAbi.Abi, networkInfo.UiPoolDataProviderAddress);
        var function = contract.GetFunction("getUserReservesData");

        return (await function.CallDeserializingToObjectAsync<UserReservesResponse>(
            networkInfo.PoolAddressesProviderAddress,
            userAddress
        )).ReservesData;
    }
}