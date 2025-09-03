using AaveClient.Extensions;
using CoinGeckoClient.Extensions;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Application;
using CryptoWatcher.HyperliquidModule.Abstractions;
using CryptoWatcher.HyperliquidModule.Extensions;
using CryptoWatcher.Infrastructure.Configs;
using CryptoWatcher.Infrastructure.Hyperliquid;
using CryptoWatcher.Infrastructure.Integrations;
using CryptoWatcher.Infrastructure.Services;
using CryptoWatcher.Infrastructure.Uniswap;
using CryptoWatcher.Integrations;
using CryptoWatcher.UniswapModule.Abstractions;
using CryptoWatcher.UniswapModule.Extensions;
using CryptoWatcher.UniswapModule.Services;
using HyperliquidClient.Extensions;
using Microsoft.Extensions.DependencyInjection;
using UniswapClient.Extensions;

namespace CryptoWatcher.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services
            .AddConfiguredHyperliquidModule()
            .AddConfiguredUniswapModule();

        services.AddScoped<ITokenEnricher, TokenEnricher>();

        services.AddSingleton<TokenService>();
        services.AddSingleton<TokenEnricher>();
        services.AddSingleton<CoinNormalizer>();
        
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

        services.AddScoped<IPoolHistorySyncRepositoryFacade, PoolHistorySyncRepositoryFacade>();

        services.AddCoinGeckoClient(provider => provider.GetRequiredService<ExternalServicesConfig>().CoinGecko);
        services.AddTransient<ICoinPriceProvider, CoinGeckoCoinPriceProvider>();

        services.AddSingleton<CoinPriceService>();

        services.AddAaveClient();
 
        services.AddSingleton<AaveProvider>();
        
        return services;
    }

    private static IServiceCollection AddConfiguredHyperliquidModule(this IServiceCollection services)
    {
        services.AddHyperLiquidClient();

        services.AddHyperliquidModule()
            .AddScoped<IHyperliquidExcelService, HyperliquidExcelService>()
            .AddScoped<IHyperliquidProvider, HyperliquidApiProvider>();

        return services;
    }

    private static IServiceCollection AddConfiguredUniswapModule(this IServiceCollection services)
    {
        services.AddUniswapClient();

        services.AddUniswapModule()
            .AddSingleton<IUniswapProvider, UniswapProvider>()
            .AddScoped<IUniswapExcelReportService, UniswapExcelReportService>();

        return services;
    }
}