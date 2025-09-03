using CryptoWatcher.Application;
using CryptoWatcher.Infrastructure.Services;
using CryptoWatcher.Infrastructure.Uniswap;
using CryptoWatcher.UniswapModule.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoWatcher.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IUniswapProvider, UniswapProvider>();
        
        services.AddScoped<ITokenEnricher, TokenEnricher>();
        services.AddScoped<TokenService>();
        services.AddScoped<CoinNormalizer>();
        return services;
    }
}