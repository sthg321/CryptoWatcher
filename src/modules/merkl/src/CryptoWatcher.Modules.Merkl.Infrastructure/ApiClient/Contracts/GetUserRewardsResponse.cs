namespace CryptoWatcher.Modules.Merkl.Infrastructure.ApiClient.Contracts;

internal class GetUserRewardsResponse
{
    public List<Reward> Rewards { get; init; } = [];
}