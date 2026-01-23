using CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Blockscout.Contracts.InternalTransactions;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Blockscout.Contracts.TransactionHistory;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Services;
using Refit;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Blockscout.Api;

/// <summary>
/// https://eth.blockscout.com/api-docs
/// blockscout core api
/// </summary>
public interface IBlockscoutApi
{
    [Get("/api/v2/addresses/{address_hash}/transactions/query")]
    Task<BlockscoutTransactionHistoryResponse> GetTransactionHistoryAsync(
        [AliasAs("address_hash")] string walletAddress,
        BlockscoutTransactionHistoryQueryParams? queryParams, CancellationToken ct = default);

    [Get("/api/v2/transactions/{transactionHash}/internal-transactions")]
    Task<BlockscoutInternalTransactionsResponse> GetInternalTransactionsAsync(string transactionHash,
        CancellationToken ct = default);

    [Get("/api/v2/transactions/{transactionHash")]
    Task<TransactionInfoResponse?> GetTransactionTimestampAsync(string transactionHash,
        CancellationToken ct = default);
}