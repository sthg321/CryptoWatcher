using System.Numerics;
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

    public async Task<CryptoToken> EnrichTokenAsync(AaveProtocolConfiguration protocol, AaveLendingPosition lendingPosition,
        CancellationToken ct = default)
    {
        var token = new Token { Address = lendingPosition.TokenAddress, Balance = (BigInteger)lendingPosition.Amount };

        return await _tokenEnricher.EnrichTokenAsync(protocol.RpcUrlWithAuthToken, token, lendingPosition.AmountUsd, ct);
    }
}