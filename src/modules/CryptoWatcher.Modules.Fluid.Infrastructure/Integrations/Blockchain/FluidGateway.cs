using System.Numerics;
using CryptoWatcher.Modules.Fluid.Application.Features.Abstractions;
using CryptoWatcher.Modules.Fluid.Application.Features.LendPositionsSynchronization.Models;
using CryptoWatcher.Modules.Fluid.Infrastructure.Integrations.Blockchain.Contracts;
using CryptoWatcher.ValueObjects;
using Nethereum.Web3;

namespace CryptoWatcher.Modules.Fluid.Infrastructure.Integrations.Blockchain;

public class FluidGateway : IFluidGateway
{
    public async Task<FluidPositionData> GetPositionDataAsync(
        string network,
        EvmAddress fTokenAddress,
        EvmAddress walletAddress)
    {
        var fTokenData = await GetFTokenDataAsync(network, fTokenAddress);
        var sharesBalance = await GetSharesBalanceAsync(network, fTokenAddress, walletAddress);

        return new FluidPositionData
        {
            FTokenAddress = fTokenAddress,
            WalletAddress = walletAddress,
            SharesBalance = sharesBalance,
            LiquidityExchangePrice = fTokenData.LiquidityExchangePrice,
            TokenExchangePrice = fTokenData.TokenExchangePrice,
            LiquidityBalance = fTokenData.LiquidityBalance,
            RewardsActive = fTokenData.RewardsActive
        };
    }

    public async Task<FluidFTokenData> GetFTokenDataAsync(string network, EvmAddress fTokenAddress)
    {
        var web3 = new Web3(network);
        var contract = web3.Eth.GetContract(GetDataAbi.Abi, fTokenAddress.Value);

        var result = await contract.GetFunction("getData").CallDeserializingToObjectAsync<GetDataOutput>();

        return new FluidFTokenData
        {
            Liquidity = EvmAddress.Create(result.Liquidity),
            LendingFactory = EvmAddress.Create(result.LendingFactory),
            LendingRewardsRateModel = EvmAddress.Create(result.LendingRewardsRateModel),
            Permit2 = EvmAddress.Create(result.Permit2),
            Rebalancer = EvmAddress.Create(result.Rebalancer),
            RewardsActive = result.RewardsActive,
            LiquidityBalance = result.LiquidityBalance,
            LiquidityExchangePrice = result.LiquidityExchangePrice,
            TokenExchangePrice = result.TokenExchangePrice
        };
    }

    public async Task<BigInteger> GetSharesBalanceAsync(
        string network,
        EvmAddress fTokenAddress,
        EvmAddress walletAddress)
    {
        var web3 = new Web3(network);

        var result = await web3.Eth.ERC20.GetContractService(fTokenAddress.Value)
            .BalanceOfQueryAsync(walletAddress.Value);

        return result;
    }
}