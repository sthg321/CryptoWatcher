using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization.UniswapV3.Models.PositionEvents;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.TransactionSynchronization;

public class UniswapTransactionEventSource : IUniswapTransactionEventSource
{
    private readonly IBlockchainGateway _blockchainGateway;
    private readonly IUniswapTransactionEnricher _transactionEnricher;

    public UniswapTransactionEventSource(IBlockchainGateway blockchainGateway,
        IUniswapTransactionEnricher transactionEnricher)
    {
        _blockchainGateway = blockchainGateway;
        _transactionEnricher = transactionEnricher;
    }

    public async Task<UniswapPositionEvent?> GetUniswapEventAsync(UniswapChainConfiguration chain, TransactionHash hash,
        CancellationToken ct = default)
    {
        var transaction = await _blockchainGateway.GetTransactionAsync(chain, hash);

        return await _transactionEnricher.TryEnrichAsync(chain, transaction, ct);
    }
}