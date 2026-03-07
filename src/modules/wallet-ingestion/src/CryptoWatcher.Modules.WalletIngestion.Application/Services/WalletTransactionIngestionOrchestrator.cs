using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.WalletIngestion.Application.Abstractions;
using CryptoWatcher.Modules.WalletIngestion.Entities;
using CryptoWatcher.Shared.Entities;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Modules.WalletIngestion.Application.Services;

public class WalletTransactionIngestionOrchestrator : IWalletTransactionIngestionOrchestrator
{
    private readonly IWalletTransactionIngestionService _transactionIngestionService;
    private readonly IWalletCheckpointRepository _checkpointRepository;
    private readonly IRepository<Wallet> _walletRepository;
    private readonly ILogger<WalletTransactionIngestionOrchestrator> _logger;
    
    public WalletTransactionIngestionOrchestrator(IWalletTransactionIngestionService transactionIngestionService,
        IWalletCheckpointRepository checkpointRepository, IRepository<Wallet> walletRepository,
        ILogger<WalletTransactionIngestionOrchestrator> logger)
    {
        _transactionIngestionService = transactionIngestionService;
        _checkpointRepository = checkpointRepository;
        _walletRepository = walletRepository;
        _logger = logger;
    }

    public async Task StartSynchronizationAsync(CancellationToken ct = default)
    {
        var wallets = await _walletRepository.ListAsync(ct);

        var checkpoints = (await _checkpointRepository.GetAllAsync(ct))
            .GroupBy(checkpoint => new { checkpoint.WalletAddress, checkpoint.ChainId })
            .ToDictionary(grouping => (grouping.Key.ChainId, grouping.Key.WalletAddress),
                grouping => grouping.SingleOrDefault());

        foreach (var wallet in wallets)
        {
            foreach (var chainId in ChainRegistry.GetAll())
            {
                var checkpoint = checkpoints.GetValueOrDefault((chainId, wallet.Address));
                if (checkpoint is null)
                {
                    _logger.LogInformation(
                        "No checkpoint found for wallet {WalletAddress} and chain {ChainId}. Creating", wallet.Address,
                        chainId);

                    checkpoint = WalletIngestionCheckpoint.Create(wallet.Address, chainId);
                }

                try
                {
                    await _transactionIngestionService.IngestAllAsync(checkpoint, ct);
                }
                catch (Exception e)
                {
                    _logger.LogError(e,
                        "Error while storing wallet transactions for wallet {WalletAddress} and chain {ChainId}.",
                        checkpoint.WalletAddress, checkpoint.ChainId);
                }
            }
        }
    }
}