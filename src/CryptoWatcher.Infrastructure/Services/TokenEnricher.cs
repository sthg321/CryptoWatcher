using CryptoWatcher.Abstractions;
using CryptoWatcher.Extensions;
using CryptoWatcher.Models;
using CryptoWatcher.Modules.Infrastructure.Shared.Integrations.Abstractions;
using CryptoWatcher.ValueObjects;
using Nethereum.Web3;

namespace CryptoWatcher.Infrastructure.Services;

/// <summary>
/// <see cref="ITokenEnricher"/>
/// </summary>
public class TokenEnricher : ITokenEnricher
{
    private readonly TokenService _tokenService;
    private readonly IWeb3Gateway _web3Gateway;
    private readonly BlockchainRegistry _blockchainRegistry;

    public TokenEnricher(TokenService tokenService, IWeb3Gateway web3Gateway, BlockchainRegistry blockchainRegistry)
    {
        _tokenService = tokenService;
        _web3Gateway = web3Gateway;
        _blockchainRegistry = blockchainRegistry;
    }

    public async ValueTask<CryptoToken> EnrichAsync(int chainId, Token token, CancellationToken ct = default)
    {
        var web3 = _web3Gateway.GetConfigured(chainId);
        var chain = _blockchainRegistry.GetNetwork(chainId);

        var tokenDecimals = await _tokenService.GetTokenDecimalsAsync(web3, token.Address, ct);
        var symbol = await _tokenService.GetTokenSymbolAsync(web3, token.Address);
        
        return new CryptoToken
        {
            Address = EvmAddress.Create(token.Address),
            Symbol = symbol,
            Amount = token.Balance.ToDecimal(tokenDecimals),
            PriceInUsd = await _tokenService.GetTokenPriceByTokenAddressAsync(chain.Name, token.Address, symbol, ct)
        };
    }


    public async ValueTask<CryptoToken> EnrichAsync(string networkName, Uri rpcAddress, Token token,
        CancellationToken ct = default)
    {
        return await EnrichTokenAsync(networkName, rpcAddress, token, null, ct);
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