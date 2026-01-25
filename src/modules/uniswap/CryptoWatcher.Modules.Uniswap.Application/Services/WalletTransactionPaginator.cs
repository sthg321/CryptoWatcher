using System.Runtime.CompilerServices;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Services;

public class WalletTransactionPaginator : IWalletTransactionPaginator
{
    private const int PageSize = 1000;

    private readonly IWalletTransactionGateway _transactionGateway;
    private readonly IEtherscanApiKeyProvider _apiKeyProvider;

    public WalletTransactionPaginator(IWalletTransactionGateway transactionGateway, IEtherscanApiKeyProvider apiKeyProvider)
    {
        _transactionGateway = transactionGateway;
        _apiKeyProvider = apiKeyProvider;
    }

    public async IAsyncEnumerable<IReadOnlyCollection<BlockchainTransaction>> PaginateWalletTransactionsAsync(
        UniswapChainConfiguration chainConfiguration,
        EvmAddress walletAddress,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var transactions = await _transactionGateway.GetWalletTransactionsAsync(new EtherscanTransactionQuery
        {
            ApiKey = _apiKeyProvider.ApiKey(),
            ChainId = chainConfiguration.ChainId,
            Page = PageSize,
            StartBlock = chainConfiguration.LastProcessedBlock,
            Address = walletAddress,
            Offset = 0
        }, ct);

        yield return transactions;

        var offset = 0;

        while (transactions.Count != 0)
        {
            offset++;

            transactions = await _transactionGateway.GetWalletTransactionsAsync(new EtherscanTransactionQuery
            {
                ChainId = chainConfiguration.ChainId,
                Offset = offset,
                Page = PageSize,
                StartBlock = chainConfiguration.LastProcessedBlock,
                Address = walletAddress,
                ApiKey = _apiKeyProvider.ApiKey()
            }, ct);

            yield return transactions;
        }
    }
}