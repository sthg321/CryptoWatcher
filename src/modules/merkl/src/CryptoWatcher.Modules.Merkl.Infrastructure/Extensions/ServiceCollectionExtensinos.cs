using CryptoWatcher.Modules.Merkl.Application.Abstractions;
using CryptoWatcher.Modules.Merkl.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoWatcher.Modules.Merkl.Infrastructure.Extensions;

public static class ServiceCollectionExtensinos
{
    public static IServiceCollection AddMerklModule(this IServiceCollection services,
        Func<IServiceProvider, Uri> uriFactory)
    {
        services.AddHttpClient<IMerklProvider, MerklProvider>((provider, client) =>
            client.BaseAddress = uriFactory(provider));

        services.AddScoped<IMerklSyncService, MerklSyncService>();
        services.AddScoped<IMerklRewardService, MerklRewardService>();
        return services;
    }
}