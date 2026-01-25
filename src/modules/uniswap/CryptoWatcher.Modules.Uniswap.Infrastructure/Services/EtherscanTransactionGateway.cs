using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Etherscan.Api;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Etherscan.Contracts.TransactionHistory;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services;

public class EtherscanTransactionGateway : IWalletTransactionGateway
{
    private readonly IEtherscanApi _etherscanApi;

    public EtherscanTransactionGateway(IEtherscanApi etherscanApi)
    {
        _etherscanApi = etherscanApi;
    }

    public async Task<IReadOnlyCollection<BlockchainTransaction>> GetWalletTransactionsAsync(
        EtherscanTransactionQuery etherscanTransactionQuery,
        CancellationToken ct = default)
    {
        var transactions = await _etherscanApi.GetAccountTransactionsAsync(etherscanTransactionQuery.MapQuery(), ct);

        if (transactions.Result.Length == 0)
        {
            return [];
        }

        return transactions.Result.Select(item => item.MapToBlockchainTransaction()).ToArray();
    }
}