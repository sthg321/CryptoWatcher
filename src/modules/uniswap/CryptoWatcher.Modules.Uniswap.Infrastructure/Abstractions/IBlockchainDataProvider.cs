using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Abstractions;

public interface IBlockchainDataProvider
{
    Task<DateTime> GetBlockTimestampAsync(UniswapChainConfiguration chain, HexBigInteger blockNumber);

    Task<TransactionReceipt> GetTransactionReceiptAsync(UniswapChainConfiguration chain,
        TransactionHash transactionHash);
}