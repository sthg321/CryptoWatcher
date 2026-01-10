using CryptoWatcher.Integrations;
using CryptoWatcher.ValueObjects;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Nethereum.Web3;

namespace CryptoWatcher.Infrastructure.Services;

public class TokenService
{
    private readonly ICoinPriceProvider _coinGeckoCoinPriceProvider;
    private readonly HybridCache _cache;
    private readonly ILogger<TokenService> _logger;

    public TokenService(ICoinPriceProvider coinGeckoCoinPriceProvider, HybridCache cache, ILogger<TokenService> logger)
    {
        _coinGeckoCoinPriceProvider = coinGeckoCoinPriceProvider;
        _cache = cache;
        _logger = logger;
    }

    public async ValueTask<string> GetTokenSymbolAsync(IWeb3 web3, EvmAddress tokenAddress)
    {
        var cacheKey = string.Format(CacheKeys.TokenSymbol.TokenSymbolByTokenAddressTemplate, tokenAddress);
        return await _cache.GetOrCreateAsync(cacheKey, web3, async (web, _) =>
                {
                    if (tokenAddress == "0x0000000000000000000000000000000000000000") // ETH in unichain
                    {
                        return "ETH";
                    }

                    try
                    {
                        var result = await web.Eth.ERC20.GetContractService(tokenAddress).SymbolQueryAsync();
                        return result;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e,"Unable to get token symbol");
                        throw;
                    }
                },
                new HybridCacheEntryOptions
                    { Expiration = TimeSpan.FromSeconds(CacheKeys.TokenSymbol.CacheLifetimeInSecond) })
            .ConfigureAwait(false);
    }

    public async ValueTask<byte> GetTokenDecimalsAsync(IWeb3 web3, EvmAddress tokenAddress, CancellationToken ct = default)
    {
        var cacheKey = string.Format(CacheKeys.TokenDecimals.TokenDecimalsByTokenAddressTemplate, tokenAddress);
        return (byte)await _cache.GetOrCreateAsync(cacheKey, web3,
            async (web, _) =>
            {
                if (tokenAddress == "0x0000000000000000000000000000000000000000") // ETH in unichain
                {
                    return 18;
                }
                
                try
                {
                    var result = await web.Eth.ERC20.GetContractService(tokenAddress).DecimalsQueryAsync();
                    return result;
                }
                catch (Exception e)
                {
                    _logger.LogError(e,"Unable to get token decimals");
                    throw;
                }
            },
            new HybridCacheEntryOptions
                { Expiration = TimeSpan.FromSeconds(CacheKeys.TokenDecimals.CacheLifetimeInSecond) },
            cancellationToken: ct);
    }

    public async ValueTask<decimal> GetTokenPriceByTokenAddressAsync(string platform, EvmAddress address,
        string symbol,
        CancellationToken ct)
    {
        // we need to cahe symbol instead of address to avoid additional requests to api
        var cacheKey = string.Format(CacheKeys.TokenPrice.TokenPriceInUsdByTokenSymbolCacheKeyTemplate, symbol);
        
        return await _cache.GetOrCreateAsync(cacheKey, address, async (s, token) =>
            {
                var result = await _coinGeckoCoinPriceProvider.GetTokenPriceInUsdAsync(platform, s, token);

                return result;
            },
            new HybridCacheEntryOptions
                { Expiration = TimeSpan.FromSeconds(CacheKeys.TokenPrice.CacheLifetimeInSecond) },
            cancellationToken: ct);
    }
}