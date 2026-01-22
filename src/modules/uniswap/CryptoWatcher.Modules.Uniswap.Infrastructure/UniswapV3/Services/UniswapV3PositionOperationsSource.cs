using CryptoWatcher.Modules.Uniswap.Application.Abstractions.OperationReaders;
using CryptoWatcher.Modules.Uniswap.Application.UniswapV3.Models.Operations;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Abstractions;
using CryptoWatcher.Modules.Uniswap.Infrastructure.UniswapV3.Abstractions;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.UniswapV3.Services;

public class UniswapV3PositionOperationsSource : IPositionOperationsSource
{
    private readonly IUniswapTransactionLogsDecoderFactory _decoderFactory;
    private readonly IBlockchainDataSource _blockchainDataSource;

    public UniswapV3PositionOperationsSource(IUniswapTransactionLogsDecoderFactory decoderFactory,
        IBlockchainDataSource blockchainDataSource)
    {
        _decoderFactory = decoderFactory;
        _blockchainDataSource = blockchainDataSource;
    }

    public async Task<PositionOperation?> GetOperationFromTransactionAsync(UniswapChainConfiguration chainConfiguration,
        TransactionHash hash,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var transactionReceipt =
            await _blockchainDataSource.GetTransactionReceiptAsync(chainConfiguration, hash);

        ct.ThrowIfCancellationRequested();

        return _decoderFactory.GetOperationFromTransaction(transactionReceipt);
    }
}