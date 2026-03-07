using CryptoWatcher.Modules.WalletIngestion.Application.Abstractions;
using CryptoWatcher.Modules.WalletIngestion.Entities;

namespace CryptoWatcher.Modules.WalletIngestion.Application.Services;

public class WalletTransactionIngestionService : IWalletTransactionIngestionService
{
    private const int ChunkSize = 100;
    
    private readonly IUnprocessedWalletTransactions _unprocessedWalletTransactions;
    private readonly IWalletTransactionProducer _producer;
    private readonly IWalletCheckpointRepository _checkpointRepository;
    private readonly TimeProvider _timeProvider;

    public WalletTransactionIngestionService(IUnprocessedWalletTransactions unprocessedWalletTransactions,
        IWalletTransactionProducer producer, IWalletCheckpointRepository checkpointRepository,
        TimeProvider timeProvider)
    {
        _unprocessedWalletTransactions = unprocessedWalletTransactions;
        _producer = producer;
        _checkpointRepository = checkpointRepository;
        _timeProvider = timeProvider;
    }

    public async Task IngestAllAsync(WalletIngestionCheckpoint checkpoint, CancellationToken ct = default)
    {
        await foreach (var batch in _unprocessedWalletTransactions
                           .GetTransactionsAsync(checkpoint, ct)
                           .Chunk(ChunkSize).WithCancellation(ct))
        {
            foreach (var transaction in batch)
            {
                await _producer.ProduceAsync(transaction, ct);
            }

            var last = batch.MaxBy(t => t.BlockNumber)!;
            checkpoint.Advance(last.Hash, last.BlockNumber, _timeProvider);
            await _checkpointRepository.SaveCheckpointsAsync(checkpoint, ct);
        }
    }
}