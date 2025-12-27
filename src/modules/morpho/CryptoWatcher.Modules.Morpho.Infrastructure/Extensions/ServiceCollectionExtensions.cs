using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoWatcher.Modules.Morpho.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMorphoModule(this IServiceCollection services,
        Func<IServiceProvider, Uri> morphoUriFactory)
    {
        services.AddScoped<IGraphQLClient>(provider =>
            new GraphQLHttpClient(morphoUriFactory(provider), new SystemTextJsonSerializer()));

        services.AddScoped<MorphoClient>();

        return services;
    }
}