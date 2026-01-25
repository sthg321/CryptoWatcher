using System.Runtime.CompilerServices;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Models;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Specifications;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Uniswap.Application.Services;

public class UniswapWalletEventSynchronizer : IUniswapWalletEventSynchronizer
{
    private const int ChunkSize = 50;

    private readonly IUniswapWalletTransactionScanner _uniswapWalletTransactionScanner;
    private readonly IUniswapLiquidityPositionEventReducer _positionEventReducer;
    private readonly IRepository<UniswapLiquidityPosition> _positionsRepository;

    public UniswapWalletEventSynchronizer(IUniswapWalletTransactionScanner uniswapWalletTransactionScanner,
        IUniswapLiquidityPositionEventReducer positionEventReducer,
        IRepository<UniswapLiquidityPosition> positionsRepository)
    {
        _uniswapWalletTransactionScanner = uniswapWalletTransactionScanner;
        _positionEventReducer = positionEventReducer;
        _positionsRepository = positionsRepository;
    }

    public async IAsyncEnumerable<WalletEventExtractionResult> SynchronizeWalletEventsAsync(
        UniswapChainConfiguration chainConfiguration,
        UniswapSynchronizationState synchronizationState,
        Wallet wallet,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var scannedTransactions = _uniswapWalletTransactionScanner
            .ScanWalletTransactionsAsync(chainConfiguration, synchronizationState, wallet.Address, ct)
            .ToBlockingEnumerable(cancellationToken: ct); // until a problem with pending transaction is fixed

        foreach (var uniswapEventBatch in scannedTransactions.Chunk(ChunkSize))
        {
            var uniswapEvents = uniswapEventBatch
                .Where(item => item.Event is not null)
                .Select(eventScanItem => eventScanItem.Event!)
                .ToArray();

            var updatedPositions = Array.Empty<UniswapLiquidityPosition>();

            if (uniswapEvents.Length > 0)
            {
                var positionIds = uniswapEvents
                    .Select(e => e.Operation.PositionId)
                    .Distinct()
                    .ToArray();

                var currentPositions =
                    await _positionsRepository.ListAsync(new LiquidityPositionByIds(positionIds), ct);

                updatedPositions = await _positionEventReducer.ApplyEventsAsync(
                    chainConfiguration,
                    wallet.Address,
                    uniswapEvents,
                    currentPositions,
                    ct);
            }

            yield return new WalletEventExtractionResult
            {
                UpdatedPositions = updatedPositions,
                LastScannedTransaction =
                    uniswapEventBatch.MaxBy(x => x.Transaction.BlockNumber)!.Transaction
            };
        }
    }
}