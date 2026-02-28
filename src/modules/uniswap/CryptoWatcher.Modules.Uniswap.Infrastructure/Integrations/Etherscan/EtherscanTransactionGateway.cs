using System.Text.Json;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Etherscan.Api;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Etherscan.Contracts.TransactionHistory;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Etherscan;

public class EtherscanTransactionGateway : IWalletTransactionGateway
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web);

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

        var transactions = response.Result.Deserialize<EtherscanTransactionHistoryItem[]>(_jsonSerializerOptions)!;

        return transactions.Select(item => item.MapToBlockchainTransaction()).ToArray();
    }
}