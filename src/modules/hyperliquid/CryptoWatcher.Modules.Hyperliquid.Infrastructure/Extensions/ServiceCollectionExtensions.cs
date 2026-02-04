using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Application.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Application.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Application.Services;
using CryptoWatcher.Modules.Hyperliquid.Infrastructure.Integrations.Hyperliquid.Api;
using CryptoWatcher.Modules.Hyperliquid.Infrastructure.Integrations.Hyperliquid.Contracts.UserNonFundingLedgerUpdates;
using CryptoWatcher.Modules.Hyperliquid.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Refit;

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

        services.AddRefitClient<IHyperliquidApi>()
            .ConfigureHttpClient((provider, client) =>
                client.BaseAddress = hyperliquidUriFactory?.Invoke(provider) ?? new Uri(BaseUrl));

        services.AddSingleton<HyperliquidVaultPositionUpdater>();
        services.AddScoped<HyperliquidVaultPositionSyncJob>();
        services.AddScoped<IHyperliquidGateway, HyperliquidApiGateway>();

        return services;
    }
}