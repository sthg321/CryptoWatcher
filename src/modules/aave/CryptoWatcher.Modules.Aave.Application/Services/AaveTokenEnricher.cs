using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Aave.Application.Abstractions;
using CryptoWatcher.Modules.Aave.Application.Models;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Aave.Application.Services;

public class AaveTokenEnricher : IAaveTokenEnricher
{
    private readonly ITokenEnricher _tokenEnricher;

    public AaveTokenEnricher(ITokenEnricher tokenEnricher)
    {
        _tokenEnricher = tokenEnricher;
    }

    public async Task<CryptoToken> EnrichTokenAsync(AaveChainConfiguration chain,
        CalculatableAaveLendingPosition position,
        CancellationToken ct = default)
    {
        var token = new Token { Address = position.TokenAddress, Balance = position.CalculateAmountWithInterest() };
        
        return await _tokenEnricher.EnrichTokenAsync(chain.RpcUrlWithAuthToken, token, position.TokenPriceInUsd, ct);
    }
}