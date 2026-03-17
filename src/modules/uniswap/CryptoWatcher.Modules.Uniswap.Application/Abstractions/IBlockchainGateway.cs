using CryptoWatcher.Modules.Contracts.Messages;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Abstractions;

public interface IBlockchainGateway
{
    Task<BlockchainTransaction> GetTransactionAsync(UniswapChainConfiguration chain, TransactionHash transactionHash);
}