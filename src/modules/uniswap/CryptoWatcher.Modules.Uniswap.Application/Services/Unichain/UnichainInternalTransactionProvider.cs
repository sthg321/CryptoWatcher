using System.Net.Http.Json;
using System.Numerics;
using CryptoWatcher.Modules.Uniswap.Application.Models;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Unichain;

public class Sender
{
    public string Hash { get; set; } = null!;
}

public class Item
{
    public string Type { get; set; } = null!;

    public string Value { get; set; } = null!;

    public Sender To { get; set; } = null!;

    public DateTimeOffset TimeStamp { get; set; }
}

public class Root
{
    public List<Item> Items { get; init; } = [];
}

public class TransactionInfoResponse
{
    public DateTimeOffset TimeStamp { get; set; }
}

public interface IUnichainInternalTransactionProvider
{
    Task<EthTransaction> GetEthAmountFromInternalTransaction(string walletAddress,
        string transactionHash,
        CancellationToken ct = default);

    Task<DateTimeOffset> GetTransactionTimestampAsync(string transactionHash,
        CancellationToken ct = default);
}

public class UnichainInternalTransactionProvider : IUnichainInternalTransactionProvider
{
    private readonly HttpClient _httpClient;

    public UnichainInternalTransactionProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DateTimeOffset> GetTransactionTimestampAsync(string transactionHash,
        CancellationToken ct = default)
    {
        var internalTransactionsResponse = await _httpClient.GetFromJsonAsync<TransactionInfoResponse>(
            $"https://unichain.blockscout.com/api/v2/transactions/{transactionHash}", ct);

        if (internalTransactionsResponse is null)
        {
            throw new InvalidOperationException(
                $"Can't find internal transaction with ETH. Transaction hash:{transactionHash}");
        }

        return internalTransactionsResponse.TimeStamp;
    }

    public async Task<EthTransaction> GetEthAmountFromInternalTransaction(string walletAddress,
        string transactionHash,
        CancellationToken ct = default)
    {
        var internalTransactionsResponse = await _httpClient.GetFromJsonAsync<Root>(
            $"https://unichain.blockscout.com/api/v2/transactions/{transactionHash}/internal-transactions", ct);

        //there should be only one internal transaction with a call and not 0 value
        var internalTransactionsWithEth =
            internalTransactionsResponse!.Items.SingleOrDefault(item =>
                item.To.Hash == walletAddress &&
                item.Value != "0" && item.Type == "call");

        if (internalTransactionsWithEth is null)
        {
            throw new InvalidOperationException(
                $"Can't find internal transaction with ETH. Transaction hash:{transactionHash}");
        }

        return new EthTransaction
        {
            Amount = BigInteger.Parse(internalTransactionsWithEth.Value),
            TimeStamp = internalTransactionsWithEth.TimeStamp
        };
    }
}