using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IUniswapTransactionClassifier
{
    Task<PositionOperationInfo?> Classify(UniswapChainConfiguration chainConfiguration, BlockchainTransaction transaction,
        CancellationToken ct = default);
}