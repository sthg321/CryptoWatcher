using System.Net.Http.Json;
using System.Numerics;
using CryptoWatcher.Modules.Merkl.Application.Abstractions;
using CryptoWatcher.Modules.Merkl.Application.Models;
using CryptoWatcher.Modules.Merkl.Infrastructure.ApiClient.Contracts;
using CryptoWatcher.Modules.Merkl.ValueObjects;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Merkl.Infrastructure;

/// <summary>
/// <remarks>api https://api.merkl.xyz/docs#description/introduction</remarks>
/// </summary>
public class MerklProvider : IMerklProvider
{
    private readonly HttpClient _httpClient;

    public MerklProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<MerklCampaignInfo[]> GetUserRewardsAsync(EvmAddress user, int chainId,
        CancellationToken ct = default)
    {
        var response =
            await _httpClient.GetFromJsonAsync<GetUserRewardsResponse[]>($"/v4/users/{user}/rewards?chainId={chainId}",
                ct);

        return response!.First().Rewards.SelectMany(reward =>
        {
            return reward.Breakdowns.Select(breakdown =>
                new MerklCampaignInfo
                {
                    CampaignId = TransactionHash.FromString(breakdown.CampaignId),
                    Claimed = BigInteger.Parse(breakdown.Claimed),
                    Pending = BigInteger.Parse(breakdown.Pending),
                    Amount =  BigInteger.Parse(breakdown.Amount),
                    ChainId = reward.Token.ChainId,
                    Reason = breakdown.Reason,
                    Asset = new Asset
                    {
                        Symbol = reward.Token.Symbol,
                        Address = EvmAddress.Create(reward.Token.Address),
                        Decimals = reward.Token.Decimals,
                        PriceInUsd = reward.Token.Price
                    }
                });
        }).ToArray();
    }
}