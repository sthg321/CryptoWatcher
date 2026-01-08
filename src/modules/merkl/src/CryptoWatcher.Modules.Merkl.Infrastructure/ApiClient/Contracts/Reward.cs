namespace CryptoWatcher.Modules.Merkl.Infrastructure.ApiClient.Contracts;

internal class Reward
{
    public string Recipient { get; init; } = null!;

    public string Amount { get; init; } = null!;

    public string Claimed { get; init; } = null!;

    public string Pending { get; init; } = null!;

    public Token Token { get; init; } = null!;

    public List<Breakdown> Breakdowns { get; init; } = [];
}