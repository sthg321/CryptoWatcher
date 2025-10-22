using System.Net.Http.Json;
using CryptoWatcher.Modules.Hyperliquid.Infrastructure.Client.UserVaultEquities.Contracts;

namespace CryptoWatcher.Modules.Hyperliquid.Infrastructure.Client.UserVaultEquities;

public interface IUserVaultEquitiesClient
{
    /// <summary>
    /// Retrieve a user's vault balance
    /// </summary>
    /// <param name="user">wallet address</param>
    /// <param name="ct"></param>
    /// <remarks>https://hyperliquid.gitbook.io/hyperliquid-docs/for-developers/api/info-endpoint?q=userFills#retrieve-a-users-vault-deposits</remarks>
    /// <returns></returns>
    Task<UserVaultEquity[]> GetUserVaultEquities(string user, CancellationToken ct = default);
}

/// <summary>
/// <inheritdoc cref="IUserVaultEquitiesClient"/>
/// </summary>
public class UserVaultEquitiesClient : IUserVaultEquitiesClient
{
    private readonly HttpClient _client;

    public UserVaultEquitiesClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<UserVaultEquity[]> GetUserVaultEquities(string user, CancellationToken ct = default)
    {
        using var response = await _client.PostAsJsonAsync("info",
            new GetUserVaultEquitiesRequest("userVaultEquities", user), cancellationToken: ct);

        response.EnsureSuccessStatusCode();
 
        return (await response.Content.ReadFromJsonAsync<UserVaultEquity[]>(cancellationToken: ct))!;
    }
}