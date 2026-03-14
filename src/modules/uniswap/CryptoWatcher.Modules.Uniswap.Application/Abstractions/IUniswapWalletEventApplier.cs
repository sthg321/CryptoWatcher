using CryptoWatcher.Modules.Contracts.Messages;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IUniswapWalletEventApplier
{
    Task<UniswapLiquidityPosition[]> ApplyEventToPositionsAsync(
        UniswapChainConfiguration chainConfiguration,
        BlockchainTransaction transaction,
        CancellationToken ct = default);
}