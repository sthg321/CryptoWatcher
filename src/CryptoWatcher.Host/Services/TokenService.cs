using CryptoWatcher.Host.Integrations;
using Microsoft.Extensions.Caching.Hybrid;
using Nethereum.Web3;

namespace CryptoWatcher.Host.Services;

public class TokenService
{
    private readonly CoinGeckoTokenPriceProvider _coinGeckoTokenPriceProvider;
    private readonly HybridCache _cache;

    public TokenService(CoinGeckoTokenPriceProvider coinGeckoTokenPriceProvider, HybridCache cache)
    {
        _coinGeckoTokenPriceProvider = coinGeckoTokenPriceProvider;
        _cache = cache;
    }

    public async ValueTask<string> GetTokenSymbolAsync(IWeb3 web3, string tokenAddress)
    {
        var cacheKey = string.Format(CacheKeys.TokenSymbol.TokenSymbolByTokenAddressTemplate, tokenAddress);
        return await _cache.GetOrCreateAsync(cacheKey, async _ =>
            {
                if (tokenAddress == "0x0000000000000000000000000000000000000000")
                {
                    return "ETH";
                }
                
                var result = await web3.Eth.ERC20.GetContractService(tokenAddress).SymbolQueryAsync();

                return result;
            },
            new HybridCacheEntryOptions
                { Expiration = TimeSpan.FromSeconds(CacheKeys.TokenSymbol.CacheLifetimeInSecond) }).ConfigureAwait(false);
    }
    
    public async ValueTask<byte> GetTokenDecimalsAsync(IWeb3 web3, string tokenAddress, CancellationToken ct = default)
    {
        var cacheKey = string.Format(CacheKeys.TokenDecimals.TokenDecimalsByTokenAddressTemplate, tokenAddress);
        return (byte)await _cache.GetOrCreateAsync(cacheKey,
            async _ =>
            {
                if (tokenAddress == "0x0000000000000000000000000000000000000000")
                {
                    return 18;
                }
                
                return await web3.Eth.ERC20.GetContractService(tokenAddress).DecimalsQueryAsync();
            },
            new HybridCacheEntryOptions
                { Expiration = TimeSpan.FromSeconds(CacheKeys.TokenDecimals.CacheLifetimeInSecond) },
            cancellationToken: ct);
    }
    
    public async ValueTask<decimal> GetTokenPriceByTokenSymbolAsync(string symbol, CancellationToken ct)
    {
        var cacheKey = string.Format(CacheKeys.TokenPrice.TokenPriceInUsdByTokenSymbolCacheKeyTemplate, symbol);
        return await _cache.GetOrCreateAsync(cacheKey, async token =>
            {
                var result = await _coinGeckoTokenPriceProvider.GetTokenPriceInUsdAsync(symbol, token);

                return result;
            },
            new HybridCacheEntryOptions
                { Expiration = TimeSpan.FromSeconds(CacheKeys.TokenPrice.CacheLifetimeInSecond) },
            cancellationToken: ct);
    }
}