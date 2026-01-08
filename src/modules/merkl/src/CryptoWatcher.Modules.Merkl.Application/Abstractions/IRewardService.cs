using CryptoWatcher.Modules.Merkl.Application.Models;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Merkl.Application.Abstractions;

public interface IRewardService
{
    Task<UniswapReward[]> GetUniswapRewardsAsync(EvmAddress walletAddress, DateOnly from, DateOnly to,
        CancellationToken ct = default);
}