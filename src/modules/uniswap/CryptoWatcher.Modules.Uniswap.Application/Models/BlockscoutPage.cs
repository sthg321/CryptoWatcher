namespace CryptoWatcher.Modules.Uniswap.Application.Models;

public class BlockscoutPage
{
    public IReadOnlyCollection<BlockscoutTransaction> Transactions { get; set; } = [];

    public BlockscoutNextPageParams? NextPageParams { get; set; }
}