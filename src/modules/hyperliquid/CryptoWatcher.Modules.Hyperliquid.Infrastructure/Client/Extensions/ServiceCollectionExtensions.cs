using CryptoWatcher.Modules.Hyperliquid.Application.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Infrastructure.Client.UserNonFundingLedgerUpdates;
using CryptoWatcher.Modules.Hyperliquid.Infrastructure.Client.UserVaultEquities;
using CryptoWatcher.Modules.Hyperliquid.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoWatcher.Modules.Hyperliquid.Infrastructure.Client.Extensions;

public static class ServiceCollectionExtensions
{
    private const string BaseUrl = "https://api.hyperliquid.xyz";

    public static void AddHyperLiquidClient(this IServiceCollection services,
        Func<IServiceProvider, Uri>? hyperliquidUriFactory = null)
    {
        services.AddHttpClient<IUserNonFundingLedgerUpdatesClient, UserNonFundingLedgerUpdatesClient>((provider,
                client) => client.BaseAddress = hyperliquidUriFactory?.Invoke(provider) ?? new Uri(BaseUrl))
            .AddStandardResilienceHandler();

        services.AddHttpClient<IUserVaultEquitiesClient, UserVaultEquitiesClient>((provider, client) =>
                client.BaseAddress = hyperliquidUriFactory?.Invoke(provider) ?? new Uri(BaseUrl))
            .AddStandardResilienceHandler();

        services.AddScoped<IHyperliquidProvider, HyperliquidApiProvider>();
        services.AddScoped<IHyperliquidApiClient, HyperliquidApiClient>();
        services.AddScoped<HyperliquidApiClient>(provider =>
            (HyperliquidApiClient)provider.GetRequiredService<IHyperliquidApiClient>());
    }
}