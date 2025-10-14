using System.Net.Http.Json;
using System.Numerics;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services;

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
    public DateTimeOffset TimeStamp { get; init; }
}

public class InternalTransactionProvider : IInternalTransactionProvider
{
    private const string CallType = "call";

    private readonly HttpClient _httpClient;

    public InternalTransactionProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DateTimeOffset> GetTransactionTimestampAsync(
        UniswapChainConfiguration chainConfiguration,
        TransactionHash transactionHash,
        CancellationToken ct = default)
    {
        var internalTransactionsResponse = await _httpClient.GetFromJsonAsync<TransactionInfoResponse>(
            $"{chainConfiguration.BlockscoutUrl}/api/v2/transactions/{transactionHash}", ct);

        if (internalTransactionsResponse is null)
        {
            throw new InvalidOperationException(
                $"Can't find internal transaction with ETH. Transaction hash:{transactionHash}");
        }

        return internalTransactionsResponse.TimeStamp;
    }

    public async Task<EthTransaction> GetEthAmountFromInternalTransaction(
        UniswapChainConfiguration chainConfiguration,
        EvmAddress walletAddress,
        TransactionHash transactionHash,
        CancellationToken ct = default)
    {
        var internalTransactionsResponse = await _httpClient.GetFromJsonAsync<Root>(
            $"{chainConfiguration.BlockscoutUrl}/api/v2/transactions/{transactionHash}/internal-transactions", ct);

        //there should be only one internal transaction with a call and not 0 value for wallet address
        var internalTransactionsWithEth = internalTransactionsResponse!.Items.SingleOrDefault(item =>
            item.To.Hash == walletAddress &&
            item.Value != "0" && item.Type == CallType);

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