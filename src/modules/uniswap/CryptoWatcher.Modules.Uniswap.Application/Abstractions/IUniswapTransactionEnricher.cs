using CryptoWatcher.Modules.Contracts.Messages;
using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.PositionEvents;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IUniswapTransactionEnricher
{
    Task<UniswapPositionEvent?> TryEnrichAsync(UniswapChainConfiguration chainConfiguration,
        BlockchainTransaction transaction,
        CancellationToken ct = default);
}