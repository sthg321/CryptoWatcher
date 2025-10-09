namespace CryptoWatcher.Modules.Uniswap.Application.Models;

public record TransactionData
{
    public required string WalletAddress { get; init; } = null!;
    
    public required string TransactionHash { get; init; } = null!;
    
    public required LiquidityEventEnrichment EventEnrichment { get; init; } = null!;
}