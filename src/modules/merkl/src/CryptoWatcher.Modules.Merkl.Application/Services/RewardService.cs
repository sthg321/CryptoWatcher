using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Merkl.Application.Abstractions;
using CryptoWatcher.Modules.Merkl.Application.Models;
using CryptoWatcher.Modules.Merkl.Entities;
using CryptoWatcher.Modules.Merkl.Specifications;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Merkl.Application.Services;

public class RewardService : IRewardService
{
    private readonly IRepository<MerklCampaign> _merklCampaignRepository;

    public RewardService(IRepository<MerklCampaign> merklCampaignRepository)
    {
        _merklCampaignRepository = merklCampaignRepository;
    }

    public async Task<UniswapReward[]> GetUniswapRewardsAsync(EvmAddress walletAddress, DateOnly from, DateOnly to,
        CancellationToken ct = default)
    {
        var campaigns =
            await _merklCampaignRepository.ListAsync(new GetRewardsSpecification(walletAddress, from, to), ct);

        return campaigns.Where(campaign => campaign.IsUniswapRewards())
            .SelectMany(campaign =>
            {
                var uniswapId = campaign.GetUniswapId();
                return campaign.Snapshots.Select(snapshot => new UniswapReward
                {
                    NftId = uniswapId,
                    Day = snapshot.Day,
                    RewardsInUsd = snapshot.NetAmount * snapshot.PriceInUsd
                });
            }).ToArray();
    }
}