using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using UniswapClient.UniswapV4.UniswapAppApiClient.Contracts;

namespace UniswapClient.UniswapV4.UniswapAppApiClient;

internal class UniswapAppApiClient
{
    private static readonly IReadOnlyCollection<int> SupportedChainIds =
    [
        1, 130, 137, 42161, 10, 8453, 56, 81457, 43114, 42220, 480, 7777777, 324
    ];

    private static readonly IReadOnlyCollection<string> SupportedProtocolVersions =
    [
        "PROTOCOL_VERSION_V4"
    ];

    private static readonly IReadOnlyCollection<string> SupportedPositionStatuses =
    [
        "POSITION_STATUS_IN_RANGE",
        "POSITION_STATUS_OUT_OF_RANGE"
    ];

    private const int PageSize = 25;
    private const string PageToken = "";

    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _client;

    public UniswapAppApiClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<ulong[]> GetPoolPositionTokenIdsAsync(string walletAddress,
        CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "v2/pools.v1.PoolsService/ListPositions");
        
        var requestBody = new GetPositionsRequest(
            walletAddress,
            SupportedChainIds,
            SupportedProtocolVersions,
            SupportedPositionStatuses,
            PageSize,
            PageToken,
            true);

        request.Headers.TryAddWithoutValidation("Origin", "https://app.uniswap.org");
        request.Content = new StringContent(
            JsonSerializer.Serialize(requestBody, JsonSerializerOptions), Encoding.UTF8,
            MediaTypeNames.Application.Json);

        using var responseMessage = await _client.SendAsync(request, ct);

        var result = await responseMessage.Content.ReadFromJsonAsync<GetPositionsResponse>(cancellationToken: ct);

        return result!.Positions
            .Where(position => position.V4Position is not null)
            .Select(position => ulong.Parse(position.V4Position!.PoolPosition.TokenId)).ToArray();
    }
}