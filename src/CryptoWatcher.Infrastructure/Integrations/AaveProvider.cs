using System.Numerics;
using AaveClient;
using CryptoWatcher.AaveModule.Abstractions;
using CryptoWatcher.AaveModule.Entities;
using CryptoWatcher.AaveModule.Models;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Infrastructure.Services;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.Shared.ValueObjects;
using AaveNetwork = CryptoWatcher.AaveModule.Models.AaveNetwork;

namespace CryptoWatcher.Infrastructure.Integrations;

public class AaveProvider : IAaveProvider
{
    private readonly IAaveApiClient _aaveApiClient;
    private readonly ITokenEnricher _tokenEnricher;

    public AaveProvider(IAaveApiClient aaveApiClient, TokenEnricher tokenEnricher)
    {
        _aaveApiClient = aaveApiClient;
        _tokenEnricher = tokenEnricher;
    }

    public async Task<List<AaveLendingPosition>> GetLendingPositionAsync(AaveNetwork aaveNetwork, Wallet wallet,
        CancellationToken ct = default)
    {
        _ = Enum.TryParse<AaveNetworkType>(aaveNetwork.Value, out var network)
            ? network
            : throw new ArgumentException(
                $"Network {aaveNetwork.Value} is not supported. Supported networks: {string.Join(", ", Enum.GetNames<AaveNetworkType>())}"
            );

        var networkInfo = NetworkRegistry.NetworkToRpcUrl[network];

        var positions =
            await _aaveApiClient.UiPoolDataProviderFetcher.GetUserReservesDataAsync(networkInfo, wallet.Address);

        var result = new List<AaveLendingPosition>();

        foreach (var userReserveData in positions)
        {
            if (userReserveData.ScaledATokenBalance > 0)
            {
                var suppliedPosition = new AaveLendingPosition
                {
                    PositionType = AavePositionType.Supplied,
                    Network = aaveNetwork,
                    Token = await EnrichToken(aaveNetwork.Value, networkInfo.RpcAddress,
                        userReserveData.ScaledATokenBalance, userReserveData.UnderlyingAsset, ct)
                };

                result.Add(suppliedPosition);
            }

            if (userReserveData.ScaledVariableDebt > 0)
            {
                var borrowedPosition = new AaveLendingPosition
                {
                    PositionType = AavePositionType.Borrowed,
                    Network = aaveNetwork,
                    Token = await EnrichToken(aaveNetwork.Value, networkInfo.RpcAddress,
                        userReserveData.ScaledVariableDebt, userReserveData.UnderlyingAsset, ct)
                };

                result.Add(borrowedPosition);
            }
        }

        return result;
    }

    private async Task<TokenInfoWithAddress> EnrichToken(string network, string rpcAddress, BigInteger amount,
        string tokenAddress,
        CancellationToken ct)
    {
        return await _tokenEnricher.EnrichTokenAsync(rpcAddress, network,
            new Token { Balance = amount, Address = tokenAddress }, ct);
    }
}