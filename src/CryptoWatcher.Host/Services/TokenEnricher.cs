using CryptoWatcher.Extensions;
using CryptoWatcher.Models;
using Nethereum.Web3;

namespace CryptoWatcher.Host.Services;

public class TokenEnricher
{
    private readonly TokenService _tokenService;

    public TokenEnricher(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async ValueTask<TokenInfoPair> EnrichAsync(IWeb3 web3, TokenPair tokenPair, CancellationToken ct = default)
    {
        return new TokenInfoPair
        {
            Token0 = await EnrichTokenAsync(web3, tokenPair.Token0, ct),
            Token1 = await EnrichTokenAsync(web3, tokenPair.Token1, ct),
        };
    }

    private async ValueTask<TokenInfo> EnrichTokenAsync(IWeb3 web3, Token token, CancellationToken ct)
    {
        var tokenDecimals = await _tokenService.GetTokenDecimalsAsync(web3, token.Address, ct);
        var symbol = await _tokenService.GetTokenSymbolAsync(web3, token.Address);
        return new TokenInfo
        {
            Address = token.Address,
            Symbol = symbol,
            Amount = token.Balance.ToDecimal(tokenDecimals),
            PriceInUsd = await _tokenService.GetTokenPriceByTokenSymbolAsync(symbol, ct)
        };
    }
}