using AaveClient;
using CryptoWatcher.Entities.Aave;
using CryptoWatcher.Infrastructure.Services;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Infrastructure.Integrations;

public class AaveProvider
{
    private readonly IAaveApiClient _aaveApiClient;
    private readonly TokenEnricher _tokenEnricher;
    
    public AaveProvider(IAaveApiClient aaveApiClient, TokenEnricher tokenEnricher)
    {
        _aaveApiClient = aaveApiClient;
        _tokenEnricher = tokenEnricher;
    }

    public async Task<LendingPosition> GetLendingPositionAsync(Wallet wallet, CancellationToken ct)
    {
        var positions =
            await _aaveApiClient.UiPoolDataProviderFetcher.GetUserReservesDataAsync(AaveNetwork.Sonic, wallet.Address);

        foreach (var position in positions)
        {
            // var token = await _tokenEnricher.EnrichTokenAsync(new Web3("https://rpc.soniclabs.com"),
            //     new Token { Address = position.UnderlyingAsset, Balance = position.ScaledATokenBalance }, ct);

        }

        return new LendingPosition();
    }
}