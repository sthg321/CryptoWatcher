using System.Text;
using System.Text.Json;
using CryptoWatcher.Modules.Morpho.Infrastructure.MorphoApiClient.Contracts;
using GraphQL;
using GraphQL.Client.Abstractions;

namespace CryptoWatcher.Modules.Morpho.Infrastructure;

public class MorphoClient
{
    private readonly IGraphQLClient _graphQlClient;

    public MorphoClient(IGraphQLClient graphQlClient)
    {
        _graphQlClient = graphQlClient;
    }

    public async Task GetUserPositionsAsync(string address, int chainId, CancellationToken ct = default)
    {
        var userByAddressRequest = new GraphQLRequest
        {
            Query = UserByAddressQuery.Query,
            OperationName = "UserByAddress",
            Variables = new
            {
                address,
                chainId
            }
        };

        var response =
            await _graphQlClient.SendQueryAsync<UserByAddressResponse>(userByAddressRequest, ct);
    }
}