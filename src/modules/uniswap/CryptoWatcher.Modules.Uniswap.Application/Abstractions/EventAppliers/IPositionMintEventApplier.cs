using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.PositionEvents;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions.EventAppliers;

public interface IPositionMintEventApplier
{
    Task<UniswapLiquidityPosition> CreatePositionAsync(
        EvmAddress walletAddress,
        MintPositionEvent mintPositionEvent,
        UniswapChainConfiguration chainConfiguration,
        DateTime timestamp,
        CancellationToken ct = default);
}