using CryptoWatcher.Application;
using CryptoWatcher.Integrations;
using Microsoft.Extensions.Caching.Hybrid;
using Nethereum.Web3;

namespace CryptoWatcher.Infrastructure.Services;

public class TokenService
{
    private readonly ICoinPriceProvider _coinGeckoCoinPriceProvider;
    private readonly HybridCache _cache;
    private readonly CoinPriceService _coinPriceService;
    private readonly CoinNormalizer _coinNormalizer;

    public TokenService(ICoinPriceProvider coinGeckoCoinPriceProvider, HybridCache cache,
        CoinPriceService coinPriceService, CoinNormalizer coinNormalizer)
    {
        _coinGeckoCoinPriceProvider = coinGeckoCoinPriceProvider;
        _cache = cache;
        _coinPriceService = coinPriceService;
        _coinNormalizer = coinNormalizer;
    }

    public async ValueTask<string> GetTokenSymbolAsync(IWeb3 web3, string tokenAddress)
    {
        var cacheKey = string.Format(CacheKeys.TokenSymbol.TokenSymbolByTokenAddressTemplate, tokenAddress);
        return await _cache.GetOrCreateAsync(cacheKey, web3, async (web, _) =>
                {
                    if (tokenAddress == "0x0000000000000000000000000000000000000000") // ETH in unichain
                    {
                        return "ETH";
                    }

                    var result = await web.Eth.ERC20.GetContractService(tokenAddress).SymbolQueryAsync();

                    return result;
                },
                new HybridCacheEntryOptions
                    { Expiration = TimeSpan.FromSeconds(CacheKeys.TokenSymbol.CacheLifetimeInSecond) })
            .ConfigureAwait(false);
    }

    public async ValueTask<byte> GetTokenDecimalsAsync(IWeb3 web3, string tokenAddress, CancellationToken ct = default)
    {
        var cacheKey = string.Format(CacheKeys.TokenDecimals.TokenDecimalsByTokenAddressTemplate, tokenAddress);
        return (byte)await _cache.GetOrCreateAsync(cacheKey, web3,
            async (web, _) =>
            {
                if (tokenAddress == "0x0000000000000000000000000000000000000000") // ETH in unichain
                {
                    return 18;
                }

                return await web.Eth.ERC20.GetContractService(tokenAddress).DecimalsQueryAsync();
            },
            new HybridCacheEntryOptions
                { Expiration = TimeSpan.FromSeconds(CacheKeys.TokenDecimals.CacheLifetimeInSecond) },
            cancellationToken: ct);
    }

    public async ValueTask<decimal> GetTokenPriceByTokenSymbolAsync(string symbol, CancellationToken ct)
    {
        var cacheKey = string.Format(CacheKeys.TokenPrice.TokenPriceInUsdByTokenSymbolCacheKeyTemplate, symbol);
        return await _cache.GetOrCreateAsync(cacheKey, symbol, async (coinSymbol, token) =>
            {
                var normalizedSymbol = _coinNormalizer.NormalizeSymbol(coinSymbol);
                var tokenId = await _coinPriceService.GetTokenIdByTokenSymbolAsync(normalizedSymbol, token);
                var result = await _coinGeckoCoinPriceProvider.GetTokenPriceInUsdAsync(tokenId, token);

                return result;
            },
            new HybridCacheEntryOptions
                { Expiration = TimeSpan.FromSeconds(CacheKeys.TokenPrice.CacheLifetimeInSecond) },
            cancellationToken: ct);
    }
}