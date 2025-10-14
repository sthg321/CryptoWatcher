using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Models;

public record TransactionData
{
    public required EvmAddress WalletAddress { get; init; } = null!;
    
    public required TransactionHash TransactionHash { get; init; } = null!;
    
    public required LiquidityEventEnrichment EventEnrichment { get; init; } = null!;
}