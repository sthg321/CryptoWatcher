using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Contracts.Messages;
using CryptoWatcher.Modules.Uniswap.Abstractions;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions;
using CryptoWatcher.Modules.Uniswap.Entities;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Modules.Uniswap.Application.Services.Synchronization.PositionsEventsSynchronization;

public class WalletTransactionConsumer : IWalletTransactionConsumer
{
    private readonly UniswapChainConfigurationService _chainConfigurationService;
    private readonly IUniswapWalletEventApplier _walletEventApplier;
    private readonly IUniswapLiquidityPositionRepository _positionRepository;
    private readonly ILogger<WalletTransactionConsumer> _logger;

    public WalletTransactionConsumer(UniswapChainConfigurationService chainConfigurationService,
        IUniswapWalletEventApplier walletEventApplier,
        ILogger<WalletTransactionConsumer> logger, IUniswapLiquidityPositionRepository positionRepository)
    {
        _chainConfigurationService = chainConfigurationService;
        _walletEventApplier = walletEventApplier;
        
        _logger = logger;
        _positionRepository = positionRepository;
    }

    public async Task ConsumeTransactionAsync(BlockchainTransaction transaction, CancellationToken ct = default)
    {
        var chain = await _chainConfigurationService.GetByIdAsync(transaction.ChainId, ct);

        var updatedPositions = await _walletEventApplier.ApplyEventToPositionsAsync(chain, transaction, ct);

        if (updatedPositions.Length > 0)
        {
            await _positionRepository.SaveAsync(updatedPositions, ct);

            _logger.LogInformation("Saved {Count} updated positions for transaction", updatedPositions.Length);
        }
    }
}
