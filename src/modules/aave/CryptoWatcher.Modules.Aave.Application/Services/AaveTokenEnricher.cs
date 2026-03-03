using CryptoWatcher.Modules.Aave.Application.Abstractions;
using CryptoWatcher.Modules.Aave.Application.Models;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Aave.Application.Services;

public class AaveTokenEnricher : IAaveTokenEnricher
{
    public Task<CryptoToken> EnrichTokenAsync(AaveProtocolConfiguration protocol, AaveLendingPosition lendingPosition,
        CancellationToken ct = default)
    {
        try
        {
            var cryptoToken = new CryptoToken
            {
                Address = lendingPosition.TokenAddress,
                Amount = lendingPosition.Amount,
                PriceInUsd = lendingPosition.TokenPriceInUsd,
                Symbol = lendingPosition.TokenSymbol
            };

            return Task.FromResult(cryptoToken);
        }
        catch (Exception exception)
        {
            return Task.FromException<CryptoToken>(exception);
        }
    }
}