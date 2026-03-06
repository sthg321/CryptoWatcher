using System.Text.Json;
using CryptoWatcher.Modules.WalletIngestion.Application.Abstractions;
using CryptoWatcher.Modules.WalletIngestion.Application.Models;
using CryptoWatcher.Modules.WalletIngestion.Infrastructure.Integrations.Etherscan.Api;
using CryptoWatcher.Modules.WalletIngestion.Infrastructure.Integrations.Etherscan.Contracts.TransactionHistory;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Modules.WalletIngestion.Infrastructure.Integrations.Etherscan;

public class EtherscanTransactionGateway : IWalletTransactionGateway
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IEtherscanApi _etherscanApi;
    private readonly ILogger<EtherscanTransactionGateway> _logger;

    public EtherscanTransactionGateway(IEtherscanApi etherscanApi, ILogger<EtherscanTransactionGateway> logger)
    {
        _etherscanApi = etherscanApi;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<BlockchainTransaction>> GetWalletTransactionsAsync(
        EtherscanTransactionQuery etherscanTransactionQuery,
        CancellationToken ct = default)
    {
        var response = await _etherscanApi.GetAccountTransactionsAsync(etherscanTransactionQuery.MapQuery(), ct);

        if (!response.IsSuccess)
        {
            _logger.LogError("Request to etherscan API failed. Status: {Status}, Message: {Message}",
                response.Status, response.Result);
            return [];
        }

        if (response.Result.GetArrayLength() == 0)
        {
            return [];
        }

        var transactions = response.Result.Deserialize<EtherscanTransactionHistoryItem[]>(JsonSerializerOptions)!;

        return transactions.Select(item => item.MapToBlockchainTransaction(etherscanTransactionQuery.ChainId))
            .ToArray();
    }
}