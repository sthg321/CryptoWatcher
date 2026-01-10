using CryptoWatcher.Modules.Merkl.Application.Models;
using CryptoWatcher.Modules.Merkl.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Merkl.Application.Abstractions;

public interface IMerklRewardService
{
    Task<IEnumerable<MerklCampaign>> GetUniswapRewardsAsync(EvmAddress walletAddress, DateOnly from, DateOnly to,
        CancellationToken ct = default);
}