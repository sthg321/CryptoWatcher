using CryptoWatcher.Abstractions;
using CryptoWatcher.Extensions;
using CryptoWatcher.Modules.Merkl.Application.Abstractions;
using CryptoWatcher.Modules.Merkl.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Merkl.Application.Services;

public class MerklSyncService
{
    private readonly IMerklProvider _provider;
    private readonly IRepository<MerklCampaign> _campaignRepo;

    public MerklSyncService(IMerklProvider provider, IRepository<MerklCampaign> campaignRepo)
    {
        _provider = provider;
        _campaignRepo = campaignRepo;
    }

    public async Task SyncRewardsAsync(EvmAddress walletAddress, int chainId,
        DateOnly day,
        CancellationToken ct = default)
    {
        var rewards = await _provider.GetUserRewardsAsync(walletAddress, chainId, ct);

        var dbCampaigns = (await _campaignRepo.ListAsync(ct))
            .ToDictionary(campaign => campaign.CampaignId);

        var result = new List<MerklCampaign>();

        foreach (var rewardCampaign in rewards)
        {
            if (!dbCampaigns.TryGetValue(rewardCampaign.CampaignId, out var campaign))
            {
                campaign = new MerklCampaign(chainId, rewardCampaign.CampaignId, walletAddress, rewardCampaign.Asset);
                result.Add(campaign);
            }

            // no new rewards
            if (rewardCampaign.Amount == rewardCampaign.Claimed && rewardCampaign.Pending == 0)
            {
                continue;
            }
            
            var pendingInUsd = rewardCampaign.Pending.ToDecimal(rewardCampaign.Asset.Decimals) * 
                               rewardCampaign.Asset.PriceInUsd;
            
            var claimedInUsd = rewardCampaign.Claimed.ToDecimal(rewardCampaign.Asset.Decimals) *
                               rewardCampaign.Asset.PriceInUsd;


            campaign.AddOrdUpdateSnapshot(day, claimedInUsd, pendingInUsd, rewardCampaign.Reason);
        }

        await _campaignRepo.BulkMergeAsync(result, ct);
    }
}