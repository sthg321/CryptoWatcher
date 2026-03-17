using CryptoWatcher.Modules.Contracts.Messages;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization;

public class UniswapWalletEventApplier : IUniswapWalletEventApplier
{
    private readonly IUniswapTransactionEnricher _transactionEnricher;
    private readonly IUniswapPositionUpdater _positionUpdater;

    public UniswapWalletEventApplier(IUniswapPositionUpdater positionUpdater,
        IUniswapTransactionEnricher transactionEnricher)
    {
        _positionUpdater = positionUpdater;
        _transactionEnricher = transactionEnricher;
    }

    public async Task<UniswapLiquidityPosition[]> ApplyEventToPositionsAsync(
        UniswapChainConfiguration chainConfiguration,
        BlockchainTransaction transaction,
        CancellationToken ct = default)
    {
        var enriched = await _transactionEnricher.TryEnrichAsync(chainConfiguration, transaction, ct);

        if (enriched is null)
        {
            return [];
        }

        return await _positionUpdater.UpdateFromEventAsync(chainConfiguration, [enriched], ct);
    }
}
