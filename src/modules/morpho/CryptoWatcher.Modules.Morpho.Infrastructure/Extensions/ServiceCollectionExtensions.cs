using CryptoWatcher.Modules.Morpho.Application.Abstractions;
using CryptoWatcher.Modules.Morpho.Application.Services;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoWatcher.Modules.Morpho.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    private const string HttpClientName = "GraphQLClient";

    public static IServiceCollection AddMorphoModule(this IServiceCollection services,
        Func<IServiceProvider, Uri> morphoUriFactory)
    {
        services.AddHttpClient(HttpClientName).AddStandardResilienceHandler();

        services.AddScoped<IGraphQLClient>(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var client = httpClientFactory.CreateClient(HttpClientName);

            return new GraphQLHttpClient(morphoUriFactory(provider), new SystemTextJsonSerializer(), client);
        });

        services.AddScoped<IMorphoClient, MorphoClient>();
        services.AddScoped<IMorphoProvider, MorphoProvider>();
        services.AddScoped<MorphoPositionsStatusService>();
        
        services.AddScoped<IMorphoMarketSynchronizer, MorphoMarketSynchronizer>();

        return services;
    }
}