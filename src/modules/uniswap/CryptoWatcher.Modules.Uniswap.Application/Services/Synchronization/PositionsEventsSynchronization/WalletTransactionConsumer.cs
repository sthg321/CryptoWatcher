using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Contracts.Messages;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization;

public class WalletTransactionConsumer : IWalletTransactionConsumer
{
    private readonly UniswapChainConfigurationService _chainConfigurationService;
    private readonly IUniswapWalletEventApplier _walletEventApplier;
    private readonly IRepository<UniswapLiquidityPosition> _repository;

    public WalletTransactionConsumer(UniswapChainConfigurationService chainConfigurationService,
        IUniswapWalletEventApplier walletEventApplier, IRepository<UniswapLiquidityPosition> repository)
    {
        _chainConfigurationService = chainConfigurationService;
        _walletEventApplier = walletEventApplier;
        _repository = repository;
    }

    public async Task ConsumeTransactionAsync(BlockchainTransaction transaction, CancellationToken ct = default)
    {
        var chain = await _chainConfigurationService.GetByIdAsync(transaction.ChainId, ct);

        var updatedPositions = await _walletEventApplier.ApplyEventToPositionsAsync(chain, transaction, ct);

        if (updatedPositions.Length > 0)
        {
            await _repository.BulkMergeAsync(updatedPositions, ct);
        }
    }
}
