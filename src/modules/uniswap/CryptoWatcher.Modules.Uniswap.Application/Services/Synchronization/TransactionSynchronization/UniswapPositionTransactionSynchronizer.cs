using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.TransactionSynchronization;

public class UniswapPositionTransactionSynchronizer : IUniswapPositionTransactionSynchronizer
{
    private readonly IUniswapPositionFromTransactionUpdater _updater;
    private readonly IUniswapLiquidityPositionRepository _repository;

    public UniswapPositionTransactionSynchronizer(IUniswapPositionFromTransactionUpdater updater,
        IUniswapLiquidityPositionRepository repository)
    {
        _updater = updater;
        _repository = repository;
    }

    public async Task SynchronizeEventFromTransactionAsync(UniswapChainConfiguration chain,
        Wallet wallet,
        TransactionHash transactionHash,
        CancellationToken ct = default)
    {
        var positions = await _updater.ApplyEventFromTransactionAsync(chain, wallet, transactionHash, ct);

        await _repository.SaveAsync(positions, ct);
    }
}