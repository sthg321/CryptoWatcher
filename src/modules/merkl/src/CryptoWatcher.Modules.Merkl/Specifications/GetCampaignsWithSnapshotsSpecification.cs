using Ardalis.Specification;
using CryptoWatcher.Modules.Merkl.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Merkl.Specifications;

public sealed class GetCampaignsWithSnapshotsSpecification : Specification<MerklCampaign>
{
    public GetCampaignsWithSnapshotsSpecification(EvmAddress walletAddress)
    {
        Query.Include(campaign => campaign.Snapshots)
            .Where(campaign => campaign.WalletAddress == walletAddress);
    }
}