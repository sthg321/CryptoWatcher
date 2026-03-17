using CryptoWatcher.Modules.Contracts.Messages;
using CryptoWatcher.Modules.WalletIngestion.Application.Abstractions;
using CryptoWatcher.Modules.WalletIngestion.Application.Models;
using CryptoWatcher.Modules.WalletIngestion.Entities;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Modules.WalletIngestion.Application.Services;

public class WalletTransactionIngestionService : IWalletTransactionIngestionService
{
    private const int ChunkSize = 100;

    private readonly IUnprocessedWalletTransactions _unprocessedWalletTransactions;
    private readonly IWalletTransactionProducer _producer;
    private readonly IWalletCheckpointRepository _checkpointRepository;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<WalletTransactionIngestionService> _logger;

    public WalletTransactionIngestionService(IUnprocessedWalletTransactions unprocessedWalletTransactions,
        IWalletTransactionProducer producer, IWalletCheckpointRepository checkpointRepository,
        TimeProvider timeProvider, ILogger<WalletTransactionIngestionService> logger)
    {
        _unprocessedWalletTransactions = unprocessedWalletTransactions;
        _producer = producer;
        _checkpointRepository = checkpointRepository;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task IngestAllAsync(WalletIngestionCheckpoint checkpoint, CancellationToken ct = default)
    {
        await foreach (var batch in _unprocessedWalletTransactions
                           .GetTransactionsAsync(checkpoint, ct)
                           .Chunk(ChunkSize).WithCancellation(ct))
        {
            BlockchainTransaction? lastProcessedTransaction = null;
            foreach (var transaction in batch)
            {
                try
                {
                    await _producer.ProduceAsync(transaction, ct);
                    lastProcessedTransaction = transaction;
                }
                catch (Exception e)
                {
                    _logger.LogError(e,
                        "Error while producing transaction. Last processed transaction: {LastProcessedTransactionHash}.",
                        lastProcessedTransaction?.Hash);

                    if (lastProcessedTransaction is null)
                    {
                        break;
                    }

                    checkpoint.Advance(lastProcessedTransaction.Hash, lastProcessedTransaction.BlockNumber,
                        _timeProvider);
                    await _checkpointRepository.SaveCheckpointsAsync(checkpoint, ct);
                    throw;
                }
            }

            var last = batch.MaxBy(t => t.BlockNumber)!;
            checkpoint.Advance(last.Hash, last.BlockNumber, _timeProvider);
            await _checkpointRepository.SaveCheckpointsAsync(checkpoint, ct);
        }
    }
}