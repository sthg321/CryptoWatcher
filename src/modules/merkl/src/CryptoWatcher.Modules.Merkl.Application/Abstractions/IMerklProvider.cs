using CryptoWatcher.Modules.Merkl.Application.Models;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Merkl.Application.Abstractions;

public interface IMerklProvider
{
    Task<MerklCampaignInfo[]> GetUserRewardsAsync(EvmAddress user, int chainId,
        CancellationToken ct = default);
}