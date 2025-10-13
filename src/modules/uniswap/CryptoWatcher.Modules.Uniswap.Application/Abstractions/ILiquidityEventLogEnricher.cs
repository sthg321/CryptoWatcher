using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface ILiquidityEventLogEnricher
{
    Task<LiquidityEventEnrichment?> EnrichLiquidityEventFromLogsAsync(UniswapChainConfiguration chain,
        EvmAddress walletAddress,
        TransactionHash transactionHash, LiquidityEventLog[] logs, CancellationToken ct = default);
}