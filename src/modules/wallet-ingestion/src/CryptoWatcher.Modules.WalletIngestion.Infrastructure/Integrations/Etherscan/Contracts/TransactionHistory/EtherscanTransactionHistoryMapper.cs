using System.Numerics;
using CryptoWatcher.Modules.WalletIngestion.Application.Models;
using Riok.Mapperly.Abstractions;

namespace CryptoWatcher.Modules.WalletIngestion.Infrastructure.Integrations.Etherscan.Contracts.TransactionHistory;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class EtherscanTransactionHistoryMapper
{
    [MapProperty(nameof(EtherscanTransactionHistoryQueryParams.StartBlock),
        nameof(EtherscanTransactionQuery.StartBlock),
        Use = nameof(MapStartBlock))]
    public static partial EtherscanTransactionHistoryQueryParams MapQuery(this EtherscanTransactionQuery query);

    [MapProperty(nameof(BlockchainTransaction.Timestamp), nameof(EtherscanTransactionHistoryItem.Timestamp),
        Use = nameof(MapDateTime))]
    public static partial BlockchainTransaction MapToBlockchainTransaction(this EtherscanTransactionHistoryItem item,
        int chainId);

    private static DateTime MapDateTime(this string timestamp) =>
        DateTimeOffset.FromUnixTimeSeconds(long.Parse(timestamp)).UtcDateTime;

    private static long MapStartBlock(this BigInteger startBlock) => (long)startBlock;
}