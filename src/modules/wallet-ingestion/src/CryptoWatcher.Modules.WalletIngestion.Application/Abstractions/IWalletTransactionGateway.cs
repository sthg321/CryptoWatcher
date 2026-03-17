using CryptoWatcher.Modules.Contracts.Messages;
using CryptoWatcher.Modules.WalletIngestion.Application.Models;

namespace CryptoWatcher.Modules.WalletIngestion.Application.Abstractions;

public interface IWalletTransactionGateway
{
    Task<IReadOnlyCollection<BlockchainTransaction>> GetWalletTransactionsAsync(
        EtherscanTransactionQuery etherscanTransactionQuery,
        CancellationToken ct = default);
}