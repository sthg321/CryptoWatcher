using CryptoWatcher.Abstractions;
using CryptoWatcher.Exceptions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.TransactionSynchronization;

public class UniswapPositionFromTransactionUpdater : IUniswapPositionFromTransactionUpdater
{
    private readonly IUniswapTransactionEventSource _transactionEventSource;
    private readonly IUniswapPositionUpdater _positionUpdater;

    public UniswapPositionFromTransactionUpdater(IUniswapTransactionEventSource transactionEventSource,
        IUniswapPositionUpdater positionUpdater)
    {
        _transactionEventSource = transactionEventSource;
        _positionUpdater = positionUpdater;
    }

    public async Task<UniswapLiquidityPosition[]> ApplyEventFromTransactionAsync(
        UniswapChainConfiguration chain,
        Wallet wallet,
        TransactionHash transactionHash,
        CancellationToken ct = default)
    {
        var uniswapEvent = await _transactionEventSource.GetUniswapEventAsync(chain, transactionHash, ct);

        if (uniswapEvent is null)
        {
            throw new InvalidOperationException("Provided transaction is not a Uniswap event");
        }
        
        return await _positionUpdater.UpdateFromEventAsync(chain, [uniswapEvent], ct);
    }
}