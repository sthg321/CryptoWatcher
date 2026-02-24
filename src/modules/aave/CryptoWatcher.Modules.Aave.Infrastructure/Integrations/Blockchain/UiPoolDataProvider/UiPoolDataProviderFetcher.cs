using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.Modules.Aave.Infrastructure.Integrations.Blockchain.UiPoolDataProvider.Contracts.ReservesData;
using CryptoWatcher.Modules.Aave.Infrastructure.Integrations.Blockchain.UiPoolDataProvider.Contracts.UserReserve;
using CryptoWatcher.ValueObjects;
using Nethereum.Contracts;
using Nethereum.Web3;

namespace CryptoWatcher.Modules.Aave.Infrastructure.Integrations.Blockchain.UiPoolDataProvider;

/// <summary>
/// Interface for fetching data related to user reserves and reserves metadata
/// from the Aave protocol's UI pool data provider.
/// </summary>
public interface IUiPoolDataProviderFetcher
{
    /// <summary>
    /// Asynchronously retrieves data related to the user's reserve positions in a specific Aave network.
    /// </summary>
    /// <param name="protocol">The network information, including addresses for the UI Pool Data Provider and Pool Addresses Provider.</param>
    /// <param name="userAddress">The address of the user whose reserve data is being queried.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a list of <c>UserReserveData</c> instances, each of which provides detailed information about the user's reserve positions.</returns>
    Task<UserReservesResponse> GetUserReservesDataAsync(AaveProtocolConfiguration protocol,
        EvmAddress userAddress);

    /// <summary>
    /// Asynchronously retrieves detailed reserves data from the Aave protocol for a specific network.
    /// </summary>
    Task<GetReservesDataOutput> GetMarketReservesDataAsync(AaveProtocolConfiguration protocol);
}

/// <summary>
/// <see cref="IUiPoolDataProviderFetcher"/>
/// </summary>
internal class UiPoolDataProviderFetcher : IUiPoolDataProviderFetcher
{
    public async Task<UserReservesResponse> GetUserReservesDataAsync(AaveProtocolConfiguration protocol,
        EvmAddress userAddress)
    {
        var web3 = new Web3(protocol.RpcUrlWithAuthToken.ToString());

        var function = GetFunction(web3, "getUserReservesData", protocol.SmartContractAddresses.UiPoolDataProviderAddress
            .Value);

        return await function.CallDeserializingToObjectAsync<UserReservesResponse>(
            protocol.SmartContractAddresses.PoolAddressesProviderAddress.Value,
            userAddress.Value
        );
    }

    public async Task<GetReservesDataOutput> GetMarketReservesDataAsync(AaveProtocolConfiguration protocol)
    {
        var web3 = new Web3(protocol.RpcUrlWithAuthToken.ToString());

        var function = GetFunction(web3, "getReservesData", protocol.SmartContractAddresses.UiPoolDataProviderAddress.Value);
       
        return await function.CallDeserializingToObjectAsync<GetReservesDataOutput>(protocol.SmartContractAddresses
            .PoolAddressesProviderAddress.Value);
    }

    private static Function GetFunction(Web3 web3, string functionName, string uiPoolDataProviderAddress)
    {
        var contract = web3.Eth.GetContract(UiPoolDataProviderFetcherAbi.Abi, uiPoolDataProviderAddress);

        return contract.GetFunction(functionName);
    }
}