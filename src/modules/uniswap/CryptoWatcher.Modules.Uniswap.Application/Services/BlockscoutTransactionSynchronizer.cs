using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;
using CryptoWatcher.Modules.Uniswap.Specifications;
using CryptoWatcher.Shared.Entities;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Modules.Uniswap.Application.Services;

public class BlockscoutTransactionSynchronizer : IBlockscoutTransactionSynchronizer
{
    private const string ModifyLiquidityMethodName = "modifyLiquidities";
    private const string CollectMethodName = "Collect";

    private readonly IBlockscoutTransactionFetcher _blockscoutTransactionFetcher;
    private readonly IUniswapCashFlowBlockRangeSynchronizer _blockRangeSynchronizer;
    private readonly IRepository<UniswapSynchronizationState> _synchronizationStateRepository;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<BlockscoutTransactionSynchronizer> _logger;

    public BlockscoutTransactionSynchronizer(IBlockscoutTransactionFetcher blockscoutTransactionFetcher,
        IUniswapCashFlowBlockRangeSynchronizer blockRangeSynchronizer,
        IRepository<UniswapSynchronizationState> synchronizationStateRepository, TimeProvider timeProvider,
        ILogger<BlockscoutTransactionSynchronizer> logger)
    {
        _blockscoutTransactionFetcher = blockscoutTransactionFetcher;
        _blockRangeSynchronizer = blockRangeSynchronizer;
        _synchronizationStateRepository = synchronizationStateRepository;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task SyncAsync(UniswapChainConfiguration chain,
        Wallet wallet, CancellationToken ct = default)
    {
        _logger.LogInformation("Begin transaction synchronization from block scout for wallet address: {WalletAddress}",
            wallet.Address);

        _logger.BeginScope("Wallet address: {WalletAddress}", wallet.Address);

        var synchronizationState = await _synchronizationStateRepository.FirstOrDefaultAsync(
            new UniswapSynchronizationStateByWalletAndChain(chain, wallet), ct);

        if (synchronizationState is null)
        {
            _logger.LogInformation("There is no synchronization state. Synchronization will start for the first time.");
        }
        else
        {
            _logger.LogInformation(
                "Last synchronized transaction from block scout:{TransactionHash} and block number: {BlockNumber}",
                synchronizationState.LastTransactionHash.Value, synchronizationState.LastBlockNumber);
        }

        await _synchronizationStateRepository.UnitOfWork.BeginTransactionAsync(ct);

        await foreach (var transaction in _blockscoutTransactionFetcher.GetTransactionsAsync(chain,
                           wallet, synchronizationState?.LastTransactionHash, ct))
        {
            try
            {
                _logger.LogInformation(
                    "Start processing transaction: {TransactionHash} with bloc number: {BlockNumber}",
                    transaction.TransactionHash.Value, transaction.BlockNumber);

                if (transaction.Method is ModifyLiquidityMethodName or CollectMethodName)
                {
                    _logger.LogInformation("Transaction is modify liquidity event. Calling sync block range");

                    await _blockRangeSynchronizer.SynchronizeBlockRangeAsync(chain, transaction.BlockNumber,
                        transaction.BlockNumber, ct);
                }

                _logger.LogInformation("Transaction method is not modify liquidity. Method is :{Method}",
                    transaction.Method);

                if (synchronizationState is null)
                {
                    synchronizationState = new UniswapSynchronizationState(chain, wallet, transaction.TransactionHash,
                        transaction.BlockNumber, _timeProvider);

                    _synchronizationStateRepository.Insert(synchronizationState);
                }

                if (synchronizationState.LastBlockNumber >= transaction.BlockNumber)
                {
                    continue;
                }

                _logger.LogInformation(
                    "Begin update synchronization state. Last transaction: {LastTransactionHash}. New transaction: {NewTransactionHash}",
                    synchronizationState.LastTransactionHash.Value, transaction.TransactionHash.Value);

                synchronizationState.UpdateLastSynchronizedTransaction(transaction.TransactionHash,
                    transaction.BlockNumber, _timeProvider);

                _synchronizationStateRepository.Update(synchronizationState);

                _logger.LogInformation("Synchronization state updated");
            }
            catch
            {
                await _synchronizationStateRepository.UnitOfWork.RollbackTransactionAsync(ct);
                throw;
            }
        }

        await _synchronizationStateRepository.UnitOfWork.SaveChangesAsync(ct);
        await _synchronizationStateRepository.UnitOfWork.CommitTransactionAsync(ct);
    }
}