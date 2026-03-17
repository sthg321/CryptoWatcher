using CryptoWatcher.Modules.Contracts.Messages;
using CryptoWatcher.Modules.WalletIngestion.Application.Models;

namespace CryptoWatcher.Modules.WalletIngestion.Application.Abstractions;

public interface IWalletTransactionProducer
{
    Task ProduceAsync(BlockchainTransaction transactions, CancellationToken ct = default);
}