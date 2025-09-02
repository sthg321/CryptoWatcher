using CryptoWatcher.Application;
using CryptoWatcher.Extensions;
using CryptoWatcher.UniswapModule.Abstractions;
using CryptoWatcher.UniswapModule.Entities;
using CryptoWatcher.UniswapModule.Models;
using Nethereum.Web3;

namespace CryptoWatcher.Infrastructure.Services;

/// <summary>
/// <see cref="ITokenEnricher"/>
/// </summary>
public class TokenEnricher : ITokenEnricher
{
    private readonly TokenService _tokenService;
    private readonly CoinNormalizer _coinNormalizer;

    public TokenEnricher(TokenService tokenService, CoinNormalizer coinNormalizer)
    {
        _tokenService = tokenService;
        _coinNormalizer = coinNormalizer;
    }

    public async ValueTask<TokenInfoPair> EnrichAsync(UniswapNetwork network, TokenPair tokenPair,
        CancellationToken ct = default)
    {
        return new TokenInfoPair
        {
            Token0 = await EnrichTokenAsync(network, tokenPair.Token0, ct),
            Token1 = await EnrichTokenAsync(network, tokenPair.Token1, ct),
        };
    }

    private async ValueTask<TokenInfoWithAddress> EnrichTokenAsync(UniswapNetwork network, Token token,
        CancellationToken ct)
    {
        var web3 = new Web3(network.RpcUrl);
        var tokenDecimals = await _tokenService.GetTokenDecimalsAsync(web3, token.Address, ct);
        var symbol = await _tokenService.GetTokenSymbolAsync(web3, token.Address);
        return new TokenInfoWithAddress
        {
            Address = token.Address,
            Symbol = symbol,
            Amount = token.Balance.ToDecimal(tokenDecimals),
            PriceInUsd = await _tokenService.GetTokenPriceByTokenSymbolAsync(symbol, ct)
        };
    }
}