using System.Threading.RateLimiting;
using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Modules.Hyperliquid.Application.Features.Reports;
using CryptoWatcher.Modules.Hyperliquid.Application.Features.Synchronization.VaultSynchronization;
using CryptoWatcher.Modules.Hyperliquid.Application.Features.Synchronization.VaultSynchronization.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Infrastructure.Integrations.Hyperliquid.Api;
using CryptoWatcher.Modules.Hyperliquid.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;
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
        services.AddKeyedScoped<IPlatformDailyReportDataProvider, HyperliquidReportDataService>(
            HyperliquidModuleKeyedService.DailyPlatformKeyService);

        services.AddRefitClient<IHyperliquidApi>()
            .ConfigureHttpClient((provider, client) =>
                client.BaseAddress = hyperliquidUriFactory?.Invoke(provider) ?? new Uri(BaseUrl))
            .AddResilienceHandler("rate-limiter", builder =>
            {
                //https://hyperliquid.gitbook.io/hyperliquid-docs/for-developers/api/rate-limits-and-user-limits
                builder.AddRateLimiter(new SlidingWindowRateLimiter(
                    new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 50,
                        Window = TimeSpan.FromMinutes(1), 
                        SegmentsPerWindow = 6,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 10
                    }));
            });

        services.AddSingleton<IUnprocessedVaultUpdatesFilter, UnprocessedVaultUpdatesFilter>();
        services.AddSingleton<IHyperliquidVaultPositionUpdater, HyperliquidVaultPositionUpdater>();
        services.AddSingleton<IHyperliquidSnapshotsUpdater, HyperliquidSnapshotsUpdater>();
        services.AddSingleton<IHyperliquidGateway, HyperliquidApiGateway>();

        services.AddScoped<IHyperliquidVaultPositionSyncJob, HyperliquidVaultPositionSyncJob>();

        return services;
    }
}