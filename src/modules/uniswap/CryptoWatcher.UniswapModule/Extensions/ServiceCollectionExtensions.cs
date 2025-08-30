using Microsoft.Extensions.DependencyInjection;

namespace CryptoWatcher.UniswapModule.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUniswapModule(this IServiceCollection services)
    {
        services.AddSingleton<IUniswapMath, UniswapMath>();
        return services;
    }
}