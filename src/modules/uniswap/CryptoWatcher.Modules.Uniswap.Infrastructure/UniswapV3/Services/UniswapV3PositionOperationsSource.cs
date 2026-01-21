using CryptoWatcher.Modules.Uniswap.Application.Abstractions.OperationReaders;
using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.OperationReaders;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Abstractions;
using CryptoWatcher.Modules.Uniswap.Infrastructure.UniswapV3.Abstractions;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.UniswapV3.Services;

public class UniswapV3PositionOperationsSource : IPositionOperationsSource
{
    private readonly IUniswapTransactionLogsDecoderFactory _decoderFactory;
    private readonly IBlockchainDataProvider _blockchainDataProvider;

    public UniswapV3PositionOperationsSource(IUniswapTransactionLogsDecoderFactory decoderFactory,
        IBlockchainDataProvider blockchainDataProvider)
    {
        _decoderFactory = decoderFactory;
        _blockchainDataProvider = blockchainDataProvider;
    }

    public async Task<PositionOperationInfo?> GetOperationFromTransactionAsync(
        UniswapChainConfiguration chainConfiguration, TransactionHash transactionHash,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        
        var transactionReceipt =
            await _blockchainDataProvider.GetTransactionReceiptAsync(chainConfiguration, transactionHash);

        ct.ThrowIfCancellationRequested();
        
        var operation = _decoderFactory.GetOperationFromTransaction(transactionReceipt);

        ct.ThrowIfCancellationRequested();

        if (operation is null)
        {
            return null;
        }
        
        return new PositionOperationInfo
        {
            Operation = operation,
            OperationDate =
                await _blockchainDataProvider.GetBlockTimestampAsync(chainConfiguration, transactionReceipt.BlockNumber)
        };
    }
}