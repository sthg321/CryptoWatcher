using CryptoWatcher.Modules.WalletIngestion.Infrastructure.Integrations.Etherscan.Contracts.TransactionHistory;
using Refit;

namespace CryptoWatcher.Modules.WalletIngestion.Infrastructure.Integrations.Etherscan.Api;

public interface IEtherscanApi
{
    /// <summary>
    /// https://docs.etherscan.io/api-reference/endpoint/txlist
    /// </summary>
    /// <returns></returns>
    [Get("/v2/api")]
    Task<EtherscanTransactionHistoryResponse> GetAccountTransactionsAsync(
        EtherscanTransactionHistoryQueryParams queryParams, CancellationToken ct = default);
}