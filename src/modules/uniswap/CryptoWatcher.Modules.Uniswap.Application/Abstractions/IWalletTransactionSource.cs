using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IWalletTransactionSource
{
    Task<IReadOnlyCollection<BlockchainTransaction>> GetWalletTransactionsAsync(
        EvmAddress walletAddress, int chainId, string apiKey, int page, int offset, CancellationToken ct = default);
}