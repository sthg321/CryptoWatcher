using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Abstractions;
using CryptoWatcher.ValueObjects;
using Nethereum.RPC.Eth.DTOs;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services;

public class Web3BlockchainDataSource : IBlockchainDataSource
{
    private readonly IWeb3Factory _web3Factory;

    public Web3BlockchainDataSource(IWeb3Factory web3Factory)
    {
        _web3Factory = web3Factory;
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