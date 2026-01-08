using Ardalis.Specification;
using CryptoWatcher.Modules.Merkl.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Merkl.Specifications;

public sealed class GetRewardsSpecification : Specification<MerklCampaign>
{
    public GetRewardsSpecification(EvmAddress walletAddress, DateOnly from, DateOnly to)
    {
        Query.Where(campaign => campaign.WalletAddress == walletAddress)
            .Include(campaign => campaign.Snapshots.Where(snapshot => snapshot.Day >= from && snapshot.Day <= to));
    }
}