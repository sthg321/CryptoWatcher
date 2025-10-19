using System.Net.Http.Json;
using System.Numerics;
using System.Text.Json.Serialization;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Client.Blockscout.Contracts;
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

public class BlockscoutResponse
{
    public List<BlockscoutTransactionItem> Items { get; init; } = [];

    [JsonPropertyName("next_page_params")] public NextPageParams? NextPageParams { get; init; }
}

public class BlockscoutTransactionItem
{
    public Sender From { get; set; } = null!;

    public string Hash { get; set; } = null!;

    public string? Method { get; set; }

    [JsonPropertyName("block_number")] public long BlockNumber { get; set; }
}

public class BlockscoutProvider : IBlockscoutProvider
{
    private const string CallType = "call";

    private readonly HttpClient _httpClient;

    public BlockscoutProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<BlockscoutPage> GetAccountTransactionsAsync(
        UniswapChainConfiguration chain,
        EvmAddress walletAddress,
        BlockscoutNextPageParams? nextPageParams,
        CancellationToken ct = default)
    {
        var query = nextPageParams is not null
            ? $"?index={nextPageParams.Index}" +
              $"&hash={nextPageParams.Hash.Value}" +
              $"&block_number={nextPageParams.BlockNumber}" +
              $"&items_count=50"
            : null;

        var blockscoutResponse = await _httpClient.GetFromJsonAsync<BlockscoutResponse>(
            $"{chain.BlockscoutUrl}/api/v2/addresses/{walletAddress.Value}/transactions{query}", cancellationToken: ct);

        var result = new BlockscoutPage
        {
            Transactions = blockscoutResponse!.Items.Select(item => new BlockscoutTransaction
            {
                Method = item.Method,
                BlockNumber = item.BlockNumber,
                TransactionHash = item.Hash
            }).ToArray(),
            NextPageParams = blockscoutResponse.NextPageParams is not null
                ? new BlockscoutNextPageParams
                {
                    BlockNumber = blockscoutResponse.NextPageParams.BlockNumber,
                    Index = blockscoutResponse.NextPageParams.Index,
                    Hash = TransactionHash.FromString(blockscoutResponse.NextPageParams.Hash)
                }
                : null
        };

        return result;
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

        // if there is no eth then it means that pool has 100% value in second token and eth percent is empty
        if (internalTransactionsWithEth is null)
        {
            return new EthTransaction
            {
                Amount = 0,
                TimeStamp = internalTransactionsResponse.Items.First().TimeStamp,
            };
        }

        return new EthTransaction
        {
            Amount = BigInteger.Parse(internalTransactionsWithEth.Value),
            TimeStamp = internalTransactionsWithEth.TimeStamp
        };
    }
}