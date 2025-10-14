using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IInternalTransactionProvider
{
    Task<EthTransaction> GetEthAmountFromInternalTransaction(UniswapChainConfiguration chainConfiguration,
        EvmAddress walletAddress,
        TransactionHash transactionHash,
        CancellationToken ct = default);

    Task<DateTimeOffset> GetTransactionTimestampAsync(UniswapChainConfiguration chainConfiguration,
        TransactionHash transactionHash,
        CancellationToken ct = default);
}