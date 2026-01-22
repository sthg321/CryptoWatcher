using System.Net.Http.Json;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services;

public class EterhscanResponse
{
    public EtherscanTransaction[] Result { get; init; } = [];
}

public class EtherscanTransaction
{
    public long Timestamp { get; set; }

    public string Hash { get; set; } = null!;

    public string FunctionName { get; set; } = null!;
    
    public string To { get; set; } = null!;
}

public class EtherscanTransactionSource : IWalletTransactionSource
{
    private const string TransactionsUrlTemplate =
        "v2/api?page={0}&offset={1}&apikey={2}&chainid={3}&module=account&action=txlist&address={4}";

    private readonly HttpClient _httpClient;

    public EtherscanTransactionSource(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyCollection<BlockchainTransaction>> GetWalletTransactionsAsync(
        EvmAddress walletAddress, int chainId, string apiKey, int page, int offset, CancellationToken ct = default)
    {
        var url = string.Format(TransactionsUrlTemplate, page, offset, apiKey, chainId, walletAddress.Value);

        var transactions = await _httpClient.GetFromJsonAsync<EterhscanResponse>(url, ct);

        if (transactions!.Result.Length == 0)
        {
            return [];
        }

        return transactions.Result.Select(transaction => new BlockchainTransaction
            {
                Hash = TransactionHash.FromString(transaction.Hash),
                FunctionName = transaction.FunctionName,
                To = EvmAddress.Create(transaction.To),
                Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(transaction.Timestamp).UtcDateTime,
            })
            .ToArray();
    }
}