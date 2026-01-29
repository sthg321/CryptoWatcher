using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.PositionEvents;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IPositionEventSource
{
    Task<PositionEvent?> GetEventFromTransactionAsync(UniswapChainConfiguration chainConfiguration,
        TransactionHash hash,
        CancellationToken ct = default);
}