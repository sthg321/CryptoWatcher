using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IBlockscoutProvider
{
    /// <summary>
    /// Asynchronously retrieves a paginated list of account transactions from the Blockscout service.
    /// </summary>
    /// <param name="chain">The configuration object containing details about the blockchain network.</param>
    /// <param name="walletAddress">The Ethereum wallet address to fetch transactions for.</param>
    /// <param name="nextPageParams">Optional pagination parameters to fetch the next page of transactions.</param>
    /// <param name="ct">A CancellationToken to observe while waiting for the operation to complete.</param>
    /// <returns>Account transaction sorted in descending order by block number </returns>
    Task<BlockscoutPage> GetAccountTransactionsAsync(
        UniswapChainConfiguration chain,
        EvmAddress walletAddress,
        BlockscoutNextPageParams? nextPageParams,
        CancellationToken ct = default);

    Task<EthTransaction> GetEthAmountFromInternalTransaction(UniswapChainConfiguration chainConfiguration,
        EvmAddress walletAddress,
        TransactionHash transactionHash,
        CancellationToken ct = default);

    Task<DateTimeOffset> GetTransactionTimestampAsync(UniswapChainConfiguration chainConfiguration,
        TransactionHash transactionHash,
        CancellationToken ct = default);
}