using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Application.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Application.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Application.Services;
using CryptoWatcher.Modules.Hyperliquid.Infrastructure.Client;
using CryptoWatcher.Modules.Hyperliquid.Infrastructure.Client.UserNonFundingLedgerUpdates;
using CryptoWatcher.Modules.Hyperliquid.Infrastructure.Client.UserVaultEquities;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoWatcher.Modules.Hyperliquid.Infrastructure.Extensions;

public static class HyperliquidModuleKeyedService
{
    public const string DailyPlatformKeyService = nameof(HyperliquidReportDataService);
}

public static class ServiceCollectionExtensions
{
    private const string BaseUrl = "https://api.hyperliquid.xyz";

    public static IServiceCollection AddHyperliquidModule(this IServiceCollection services,
        Func<IServiceProvider, Uri>? hyperliquidUriFactory = null)
    {
        services.AddScoped<IDailyPositionPerformanceSynchronizer, HyperliquidDailyPositionPerformanceSynchronizer>();
        services.AddScoped<IHyperliquidPositionsSyncService, HyperliquidPositionsSyncService>();
        services.AddKeyedScoped<IPlatformDailyReportDataProvider, HyperliquidReportDataService>(
            HyperliquidModuleKeyedService.DailyPlatformKeyService);

        services.AddHttpClient<IUserNonFundingLedgerUpdatesClient, UserNonFundingLedgerUpdatesClient>((provider,
                client) => client.BaseAddress = hyperliquidUriFactory?.Invoke(provider) ?? new Uri(BaseUrl))
            .AddStandardResilienceHandler();

        services.AddHttpClient<IUserVaultEquitiesClient, UserVaultEquitiesClient>((provider, client) =>
                client.BaseAddress = hyperliquidUriFactory?.Invoke(provider) ?? new Uri(BaseUrl))
            .AddStandardResilienceHandler();

        services.AddScoped<IHyperliquidApiClient, HyperliquidApiClient>();
        services.AddScoped<HyperliquidApiClient>(provider =>
            (HyperliquidApiClient)provider.GetRequiredService<IHyperliquidApiClient>());

        return services;
    }
}