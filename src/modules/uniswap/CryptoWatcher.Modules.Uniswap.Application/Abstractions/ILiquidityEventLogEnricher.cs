using CryptoWatcher.Modules.Uniswap.Application.Models;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface ILiquidityEventLogEnricher
{
    Task<LiquidityEventEnrichment?> EnrichLiquidityEventFromLogsAsync(string walletAddress,
        string transactionHash, LiquidityEventLog[] logs, CancellationToken ct = default);
}