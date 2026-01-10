using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Merkl.Application.Abstractions;
using CryptoWatcher.Modules.Merkl.Entities;
using CryptoWatcher.Modules.Merkl.Specifications;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Merkl.Application.Services;

public class MerklRewardService : IMerklRewardService
{
    private readonly IRepository<MerklCampaign> _merklCampaignRepository;

    public MerklRewardService(IRepository<MerklCampaign> merklCampaignRepository)
    {
        _merklCampaignRepository = merklCampaignRepository;
    }

    public async Task<IEnumerable<MerklCampaign>> GetUniswapRewardsAsync(EvmAddress walletAddress, DateOnly from,
        DateOnly to,
        CancellationToken ct = default)
    {
        var campaigns =
            await _merklCampaignRepository.ListAsync(new GetRewardsSpecification(walletAddress, from, to), ct);

        return campaigns.Where(campaign => campaign.IsUniswapRewards());
    }
}