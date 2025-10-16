using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Aave.Abstractions;
using CryptoWatcher.Modules.Aave.Models;
using CryptoWatcher.Shared.ValueObjects;

namespace CryptoWatcher.Modules.Aave.Services;

internal interface IAaveTokenEnricher
{
    Task<TokenInfo> GetEnrichedTokenInfoAsync(AaveNetwork network,
        CalculatableAaveLendingPosition position,
        CancellationToken ct = default);
}

internal class AaveTokenEnricher : IAaveTokenEnricher
{
    private readonly IAaveMainnetProvider _aaveMainnetProvider;
    private readonly ITokenEnricher _tokenEnricher;

    public AaveTokenEnricher(IAaveMainnetProvider aaveMainnetProvider, ITokenEnricher tokenEnricher)
    {
        _aaveMainnetProvider = aaveMainnetProvider;
        _tokenEnricher = tokenEnricher;
    }

    public async Task<TokenInfo> GetEnrichedTokenInfoAsync(AaveNetwork network,
        CalculatableAaveLendingPosition position,
        CancellationToken ct = default)
    {
        var token = new Token { Address = position.TokenAddress, Balance = position.CalculateAmountWithInterest() };

        var mainnetAddress = _aaveMainnetProvider.GetMainnetAddressByNetworkName(network);

        return await _tokenEnricher.EnrichTokenAsync(mainnetAddress, token, position.TokenPriceInUsd, ct);
    }
}