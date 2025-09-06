using AaveClient.Pool;
using AaveClient.UiPoolDataProvider;
using Microsoft.Extensions.DependencyInjection;

namespace AaveClient.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddAaveClient(this IServiceCollection services)
    {
        services.AddSingleton<IPoolFetcher, PoolFetcher>();
        services.AddSingleton<IUiPoolDataProviderFetcher, UiPoolDataProviderFetcher>();

        services.AddSingleton<IAaveApiClient, AaveApiClient>();
    }
}