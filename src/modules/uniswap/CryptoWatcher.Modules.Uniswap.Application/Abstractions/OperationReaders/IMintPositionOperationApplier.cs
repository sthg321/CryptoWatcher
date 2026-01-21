using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions.OperationReaders;

public interface IMintPositionOperationApplier
{
    Task<UniswapLiquidityPosition> ReadOperationAsync(EvmAddress walletAddress,
        PositionOperationInfo mintPositionOperation,
        UniswapChainConfiguration chainConfiguration, CancellationToken ct = default);
}