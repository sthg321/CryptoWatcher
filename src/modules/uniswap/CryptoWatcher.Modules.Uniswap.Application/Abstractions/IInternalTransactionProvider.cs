using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IInternalTransactionProvider
{
    Task<EthTransaction> GetEthAmountFromInternalTransaction(EvmAddress walletAddress,
        TransactionHash transactionHash,
        CancellationToken ct = default);

    Task<DateTimeOffset> GetTransactionTimestampAsync(TransactionHash transactionHash,
        CancellationToken ct = default);
}