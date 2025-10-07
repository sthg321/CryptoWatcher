using System.Net.Http.Json;
using System.Numerics;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Unichain;

public class Item
{
    public string Type { get; set; } = null!;

    public string Value { get; set; } = null!;
}

public class Root
{
    public List<Item> Items { get; init; } = [];
}

public interface IUnichainInternalTransactionProvider
{
    Task<BigInteger> GetEthAmountFromInternalTransaction(string transactionHash,
        CancellationToken ct = default);
}

public class UnichainInternalTransactionProvider : IUnichainInternalTransactionProvider
{
    private readonly HttpClient _httpClient;

    public UnichainInternalTransactionProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<BigInteger> GetEthAmountFromInternalTransaction(string transactionHash,
        CancellationToken ct = default)
    {
        var internalTransactionsResponse = await _httpClient.GetFromJsonAsync<Root>(
            $"https://unichain.blockscout.com/api/v2/transactions/{transactionHash}/internal-transactions", ct);

        //there should be only one internal transaction with a call and not 0 value
        var internalTransactionsWithEth =
            internalTransactionsResponse!.Items.SingleOrDefault(item => item.Value != "0" && item.Type == "call");

        if (internalTransactionsWithEth is null)
        {
            throw new InvalidOperationException(
                $"Can't find internal transaction with ETH. Transaction hash:{transactionHash}");
        }

        return BigInteger.Parse(internalTransactionsWithEth.Value);
    }
}