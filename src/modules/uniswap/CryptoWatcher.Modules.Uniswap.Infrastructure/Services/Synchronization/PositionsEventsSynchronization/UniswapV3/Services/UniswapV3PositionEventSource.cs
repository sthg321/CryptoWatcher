using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions.EventAppliers;
using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.PositionEvents;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Integrations.Blockchain.Api;
using CryptoWatcher.Modules.Uniswap.Infrastructure.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Abstractions;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Infrastructure.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Services;

public class UniswapV3PositionEventSource : IPositionEventSource
{
    private readonly IUniswapTransactionLogsDecoderFactory _decoderFactory;
    private readonly IWeb3BlockchainApi _blockchainApi;

    public UniswapV3PositionEventSource(IUniswapTransactionLogsDecoderFactory decoderFactory,
        IWeb3BlockchainApi blockchainApi)
    {
        _decoderFactory = decoderFactory;
        _blockchainApi = blockchainApi;
    }

    public async Task<PositionEvent?> GetEventFromTransactionAsync(UniswapChainConfiguration chainConfiguration,
        TransactionHash hash,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var transactionReceipt =
            await _blockchainApi.GetTransactionReceiptAsync(chainConfiguration, hash);

        ct.ThrowIfCancellationRequested();

        return _decoderFactory.DecodeEventFromTransaction(transactionReceipt);
    }
}