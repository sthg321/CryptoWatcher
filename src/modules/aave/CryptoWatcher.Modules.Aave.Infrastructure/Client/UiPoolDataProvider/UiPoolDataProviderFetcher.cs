using CryptoWatcher.Modules.Aave.Application.Models;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.Modules.Aave.Infrastructure.Client.UiPoolDataProvider.Contracts.ReservesData;
using CryptoWatcher.Modules.Aave.Infrastructure.Client.UiPoolDataProvider.Contracts.UserReserve;
using CryptoWatcher.ValueObjects;
using Nethereum.Contracts;
using Nethereum.Web3;

namespace CryptoWatcher.Modules.Aave.Infrastructure.Client.UiPoolDataProvider;

/// <summary>
/// <see cref="IUiPoolDataProviderFetcher"/>
/// </summary>
internal class UiPoolDataProviderFetcher : IUiPoolDataProviderFetcher
{
    public async Task<IReadOnlyCollection<UserReserve>> GetUserReservesDataAsync(AaveChainConfiguration chain,
        EvmAddress userAddress)
    {
        var web3 = new Web3(chain.RpcUrlWithAuthToken.ToString());

        var function = GetFunction(web3, "getUserReservesData", chain.SmartContractAddresses.UiPoolDataProviderAddress
            .Value);

        var result = await function.CallDeserializingToObjectAsync<UserReservesResponse>(
            chain.SmartContractAddresses.PoolAddressesProviderAddress.Value,
            userAddress.Value
        );

        return result.ReservesData.Select(data => new UserReserve
        {
            UnderlyingAsset = EvmAddress.Create(data.UnderlyingAsset),
            ScaledATokenBalance = data.ScaledATokenBalance,
            ScaledVariableDebt = data.ScaledVariableDebt,
            IsCollateral = data.UsageAsCollateralEnabled
        }).ToArray();
    }

    public async Task<MarketReserveOutput> GetMarketReservesDataAsync(AaveChainConfiguration chain)
    {
        var web3 = new Web3(chain.RpcUrlWithAuthToken.ToString());

        var function = GetFunction(web3, "getReservesData", chain.SmartContractAddresses.UiPoolDataProviderAddress.Value);
       
        var result = await function.CallDeserializingToObjectAsync<GetReservesDataOutput>(chain.SmartContractAddresses
            .PoolAddressesProviderAddress.Value);

        return new MarketReserveOutput
        {
            NetworkBaseTokenPriceDecimals = (byte)result.BaseCurrencyInfo.NetworkBaseTokenPriceDecimals,
            AggregatedMarketReserveData = result.ReservesData.Select(data => new AggregatedMarketReserveData
            {
                UnderlyingAsset = EvmAddress.Create(data.UnderlyingAsset),
                Decimals = data.Decimals,
                LiquidityIndex = data.LiquidityIndex,
                LiquidationLtv = data.ReserveLiquidationThreshold,
                PriceInMarketReferenceCurrency = data.PriceInMarketReferenceCurrency,
                VariableBorrowIndex = data.VariableBorrowIndex,
                ReserveLiquidationThreshold = (ushort)data.ReserveLiquidationThreshold
            }).ToArray(),
        };
    }

    private static Function GetFunction(IWeb3 web3, string functionName, string uiPoolDataProviderAddress)
    {
        var contract = web3.Eth.GetContract(UiPoolDataProviderFetcherAbi.Abi, uiPoolDataProviderAddress);

        return contract.GetFunction(functionName);
    }
}