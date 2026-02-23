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

    public async Task<CryptoToken> EnrichTokenAsync(AaveChainConfiguration chain,
        EvmAddress assetAddress,
        BigInteger amount,
        decimal priceInUsd,
        CancellationToken ct = default)
    {
        var token = new Token { Address = assetAddress, Balance = amount };

        return await _tokenEnricher.EnrichTokenAsync(chain.RpcUrlWithAuthToken, token, priceInUsd, ct);
    }

    public async Task<CryptoToken> EnrichTokenAsync(AaveChainConfiguration chain, AaveLendingPosition lendingPosition,
        CancellationToken ct = default)
    {
        var token = new Token { Address = lendingPosition.TokenAddress, Balance = (BigInteger)lendingPosition.Amount };

        return await _tokenEnricher.EnrichTokenAsync(chain.RpcUrlWithAuthToken, token, lendingPosition.AmountUsd, ct);
    }
}