using CryptoWatcher.Abstractions;
using CryptoWatcher.Extensions;
using CryptoWatcher.ValueObjects;
using Nethereum.Web3;

namespace CryptoWatcher.Infrastructure.Services;

/// <summary>
/// <see cref="ITokenEnricher"/>
/// </summary>
public class TokenEnricher : ITokenEnricher
{
    private readonly TokenService _tokenService;

    public TokenEnricher(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async ValueTask<TokenInfoPair> EnrichAsync(string networkName, Uri rpcAddress, TokenPair tokenPair,
        CancellationToken ct = default)
    {
        return new TokenInfoPair
        {
            Token0 = await EnrichTokenAsync(networkName, rpcAddress, tokenPair.Token0, null, ct),
            Token1 = await EnrichTokenAsync(networkName, rpcAddress, tokenPair.Token1, null, ct),
        };
    }

    public async ValueTask<CryptoToken> EnrichTokenAsync(Uri rpcAddress, Token token, decimal priceInUsd,
        CancellationToken ct)
    {
        return await EnrichTokenAsync(null!, rpcAddress, token, priceInUsd, ct);
    }

    private async ValueTask<CryptoToken> EnrichTokenAsync(string platform, Uri rpcAddress, Token token,
        decimal? priceInUsd = null,
        CancellationToken ct = default)
    {
        var web3 = new Web3(rpcAddress.ToString());
        var tokenDecimals = await _tokenService.GetTokenDecimalsAsync(web3, token.Address, ct);
        var symbol = await _tokenService.GetTokenSymbolAsync(web3, token.Address);
        return new CryptoToken
        {
            Address = EvmAddress.Create(token.Address),
            Symbol = symbol,
            Amount = token.Balance.ToDecimal(tokenDecimals),
            PriceInUsd = priceInUsd ??
                         await _tokenService.GetTokenPriceByTokenAddressAsync(platform, token.Address, symbol, ct)
        };
    }
}