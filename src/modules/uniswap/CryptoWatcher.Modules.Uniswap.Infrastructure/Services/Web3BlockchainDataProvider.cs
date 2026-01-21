using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Abstractions;
using CryptoWatcher.ValueObjects;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services;

public class Web3BlockchainDataProvider : IBlockchainDataProvider
{
    private readonly IWeb3Factory _web3Factory;

    public Web3BlockchainDataProvider(IWeb3Factory web3Factory)
    {
        _web3Factory = web3Factory;
    }

    public async Task<DateTime> GetBlockTimestampAsync(UniswapChainConfiguration chain, HexBigInteger blockNumber)
    {
        var web3 = _web3Factory.GetWeb3(chain);

        var block = await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(blockNumber);

        return DateTimeOffset.FromUnixTimeMilliseconds((long)block.Timestamp.Value).UtcDateTime;
    }

    public async Task<TransactionReceipt> GetTransactionReceiptAsync(UniswapChainConfiguration chain,
        TransactionHash transactionHash)
    {
        var web3 = _web3Factory.GetWeb3(chain);

        return await web3.Eth.Transactions
            .GetTransactionReceipt
            .SendRequestAsync(transactionHash);
    }
}