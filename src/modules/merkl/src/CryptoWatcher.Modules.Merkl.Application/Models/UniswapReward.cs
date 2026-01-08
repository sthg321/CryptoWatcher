namespace CryptoWatcher.Modules.Merkl.Application.Models;

public record UniswapReward
{
    public ulong NftId { get; init; }

    public DateOnly Day { get; init; }

    public decimal RewardsInUsd { get; init; }

    public record UniswapRewardKey(ulong NftId, DateOnly Day);
}