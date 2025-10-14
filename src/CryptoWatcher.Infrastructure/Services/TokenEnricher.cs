using System.Numerics;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Application;
using CryptoWatcher.Extensions;
using CryptoWatcher.Shared.ValueObjects;
using CryptoWatcher.ValueObjects;
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

    public async ValueTask<TokenInfoPair> EnrichAsync(string rpcAddress, TokenPair tokenPair,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(rpcAddress);
        ArgumentNullException.ThrowIfNull(tokenPair);

        return new TokenInfoPair
        {
            Token0 = await EnrichTokenAsync(rpcAddress, tokenPair.Token0, ct),
            Token1 = await EnrichTokenAsync(rpcAddress, tokenPair.Token1, ct),
        };
    }

    public async ValueTask<TokenInfoWithAddress> EnrichTokenAsync(string rpcAddress, Token token,
        CancellationToken ct)
    {
        var web3 = new Web3(rpcAddress);
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

    public async ValueTask<TokenInfoWithAddress> EnrichTokenAsync(string rpcAddress, Token token, decimal priceInUsd,
        CancellationToken ct)
    {
        var web3 = new Web3(rpcAddress);
        var tokenDecimals = await _tokenService.GetTokenDecimalsAsync(web3, token.Address, ct);
        var symbol = await _tokenService.GetTokenSymbolAsync(web3, token.Address);
        return new TokenInfoWithAddress
        {
            Address = token.Address,
            Symbol = symbol,
            Amount = token.Balance.ToDecimal(tokenDecimals),
            PriceInUsd = priceInUsd
        };
    }
    
    public async ValueTask<TokenInfoWithAddress> EnrichTokenAsync(string rpcAddress, string platform, Token token,
        CancellationToken ct)
    {
        var web3 = new Web3(rpcAddress);
        var tokenDecimals = await _tokenService.GetTokenDecimalsAsync(web3, token.Address, ct);
        var symbol = await _tokenService.GetTokenSymbolAsync(web3, token.Address);
        return new TokenInfoWithAddress
        {
            Address = token.Address,
            Symbol = symbol,
            Amount = token.Balance.ToDecimal(tokenDecimals),
            PriceInUsd = await _tokenService.GetTokenPriceByTokenAddressAsync(platform, token.Address, ct)
        };
    }
}