using System.Threading.RateLimiting;
using CryptoWatcher.Models;
using CryptoWatcher.Modules.Infrastructure.Shared.Integrations;
using CryptoWatcher.Modules.Infrastructure.Shared.Integrations.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace CryptoWatcher.Modules.Infrastructure.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedModule(this IServiceCollection services)
    {
        services.AddSingleton<BlockchainRegistry>();
        services.AddSingleton<IWeb3Gateway, Web3Gateway>();
        services.AddSingleton<IWeb3RpcHostProvider, Web3RpcHostProvider>();

        services.AddSingleton<DrpcConfig>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var config = new DrpcConfig();
            configuration.Bind(config);

            return config;
        });

        services.AddHttpClient(Web3Gateway.HttpClientName)
            .AddResilienceHandler(Web3Gateway.HttpClientName, (builder, context) =>
            {
                var config = context.ServiceProvider.GetRequiredService<DrpcConfig>();
                builder.AddRateLimiter(new SlidingWindowRateLimiter(new SlidingWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = config.PermitLimit,
                    QueueLimit = config.QueueLimit,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    SegmentsPerWindow = config.SegmentPerWindow,
                    Window = config.Window
                }));
            });

        return services;
    }
}