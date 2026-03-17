using CryptoWatcher.Modules.Contracts.Messages;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Blockchain.Api;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Blockchain;

public class Web3BlockchainGateway : IBlockchainGateway
{
    private readonly IWeb3BlockchainApi _blockchainApi;

    public Web3BlockchainGateway(IWeb3BlockchainApi blockchainApi)
    {
        _blockchainApi = blockchainApi;
    }

    public async Task<BlockchainTransaction> GetTransactionAsync(UniswapChainConfiguration chain,
        TransactionHash transactionHash)
    {
        var transactionReceipt = await _blockchainApi.GetTransactionReceiptAsync(chain, transactionHash);

        var transactionTimestamp =
            await _blockchainApi.GetTransactionTimestampAsync(chain, transactionReceipt.BlockNumber);

        return new BlockchainTransaction
        {
            BlockNumber = transactionReceipt.BlockNumber,
            ChainId = chain.ChainId,
            Hash = transactionHash,
            To = EvmAddress.Create(transactionReceipt.To),
            FunctionName = null,
            Timestamp = transactionTimestamp,
            From = EvmAddress.Create(transactionReceipt.From),
        };
    }
}