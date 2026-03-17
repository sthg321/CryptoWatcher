using System.Runtime.CompilerServices;
using CryptoWatcher.Modules.Contracts.Messages;
using CryptoWatcher.Modules.WalletIngestion.Application.Abstractions;
using CryptoWatcher.Modules.WalletIngestion.Application.Models;
using CryptoWatcher.Modules.WalletIngestion.Entities;

namespace CryptoWatcher.Modules.WalletIngestion.Application.Services;

public class WalletTransactionPaginator : IWalletTransactionPaginator
{
    private const int PageSize = 1000;

    private readonly IWalletTransactionGateway _transactionGateway;
    private readonly IEtherscanApiKeyProvider _apiKeyProvider;

    public WalletTransactionPaginator(IWalletTransactionGateway transactionGateway,
        IEtherscanApiKeyProvider apiKeyProvider)
    {
        _transactionGateway = transactionGateway;
        _apiKeyProvider = apiKeyProvider;
    }

    public async IAsyncEnumerable<IReadOnlyCollection<BlockchainTransaction>> PaginateWalletTransactionsAsync(
        WalletIngestionCheckpoint checkpoint,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var transactions = await _transactionGateway.GetWalletTransactionsAsync(new EtherscanTransactionQuery
        {
            ApiKey = _apiKeyProvider.ApiKey(),
            ChainId = checkpoint.ChainId,
            Page = PageSize,
            StartBlock = checkpoint.LastPublishedBlockNumber,
            Address = checkpoint.WalletAddress,
            Offset = 0
        }, ct);

        yield return transactions;

        var offset = 0;

        while (transactions.Count != 0)
        {
            offset++;

            transactions = await _transactionGateway.GetWalletTransactionsAsync(new EtherscanTransactionQuery
            {
                ApiKey = _apiKeyProvider.ApiKey(),
                ChainId = checkpoint.ChainId,
                Page = PageSize,
                StartBlock = checkpoint.LastPublishedBlockNumber,
                Address = checkpoint.WalletAddress,
                Offset = offset
            }, ct);

            yield return transactions;
        }
    }
}