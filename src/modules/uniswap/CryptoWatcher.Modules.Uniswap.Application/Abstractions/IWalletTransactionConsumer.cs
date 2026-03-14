using CryptoWatcher.Modules.Contracts.Messages;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IWalletTransactionConsumer
{
    Task ConsumeTransactionAsync(BlockchainTransaction transaction, CancellationToken ct = default);
}