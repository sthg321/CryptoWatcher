using CryptoWatcher.AaveModule.Abstractions;
using CryptoWatcher.AaveModule.Entities;
using CryptoWatcher.AaveModule.Models;
using CryptoWatcher.AaveModule.Specifications;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Shared.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CryptoWatcher.AaveModule.Services;

/// <summary>
/// Provides functionality to synchronize Aave lending positions for cryptocurrency wallets.
/// This service is responsible for fetching, updating, and persisting Aave positions data
/// in the context of a given wallet.
/// </summary>
public interface IAavePositionsSyncService
{
    /// <summary>
    /// Synchronizes the Aave lending positions for a given wallet. This method fetches
    /// the current lending positions from all supported Aave networks, processes the data,
    /// and updates the repository storage accordingly.
    /// </summary>
    /// <param name="network"></param>
    /// <param name="wallet">The wallet entity containing the address to fetch lending positions for.</param>
    /// <param name="syncDay"></param>
    /// <param name="ct">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<List<AavePosition>> SyncPositionsAsync(AaveNetwork network, Wallet wallet, DateOnly syncDay,
        CancellationToken ct = default);
}

internal class AavePositionsSyncService : IAavePositionsSyncService
{
    private readonly IAaveProvider _aaveProvider;
    private readonly IAaveTokenEnricher _aaveTokenEnricher;
    private readonly IRepository<AavePosition> _aavePositionRepository;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<AavePositionsSyncService> _logger;

    public AavePositionsSyncService(IAaveProvider aaveProvider, IAaveTokenEnricher aaveTokenEnricher,
        IRepository<AavePosition> aavePositionRepository, TimeProvider timeProvider,
        ILogger<AavePositionsSyncService>? logger = null)
    {
        _aaveProvider = aaveProvider;
        _aaveTokenEnricher = aaveTokenEnricher;
        _aavePositionRepository = aavePositionRepository;
        _timeProvider = timeProvider;

        _logger = logger ?? NullLogger<AavePositionsSyncService>.Instance;
    }

    public async Task<List<AavePosition>> SyncPositionsAsync(
        AaveNetwork network,
        Wallet wallet, DateOnly syncDay,
        CancellationToken ct = default)
    {
        var existedPositions = await _aavePositionRepository.ListAsync(
            new AavePositionsWithSnapshotsSpecification(wallet.Address, syncDay, syncDay), ct);

        _logger.LogExistedPositionsForWalletCount(wallet.Address, existedPositions.Count);

        var result = new List<AavePosition>();

        var lendingPositions = await _aaveProvider.GetLendingPositionAsync(network, wallet, ct);

        _logger.LogFetchedPositionsForNetworkCount(network.Name, lendingPositions.Count);
        
        foreach (var lendingPosition in lendingPositions)
        {
            if (lendingPosition is EmptyAaveLendingPosition)
            {
                foreach (var position in existedPositions.Where(position =>
                             position.TokenAddress == lendingPosition.TokenAddress))
                {
                    position.ClosePosition(syncDay);
                    result.Add(position);

                    _logger.LogPositionClosed(position.Id, position.TokenAddress);
                }

                continue;
            }

            var calculatableAaveLendingPosition = lendingPosition as CalculatableAaveLendingPosition ??
                                                  throw new InvalidOperationException(
                                                      "To calculate position amount, lending position must inherit from CalculatableAaveLendingPosition class");

            var tokenInfo =
                await _aaveTokenEnricher.GetEnrichedTokenInfoAsync(network, calculatableAaveLendingPosition, ct);

            var positionType = calculatableAaveLendingPosition.DeterminePositionType();

            var currentPosition = existedPositions.FirstOrDefault(position =>
                position.TokenAddress == lendingPosition.TokenAddress && position.PositionType == positionType);

            if (currentPosition is null)
            {
                currentPosition =
                    new AavePosition(network, wallet, positionType, lendingPosition.TokenAddress, syncDay);

                _aavePositionRepository.Insert(currentPosition);

                _logger.LogCreateAavePosition(currentPosition.TokenAddress, tokenInfo);
            }

            else
            {
                _aavePositionRepository.Update(currentPosition);

                _logger.LogUpdateAavePosition(currentPosition.TokenAddress, tokenInfo);
            }
            
            var positionScaleAmount = calculatableAaveLendingPosition.CalculatePositionScaleInToken();
            currentPosition.AddOrUpdateSnapshot(tokenInfo, positionScaleAmount, syncDay, _timeProvider);
            
            result.Add(currentPosition);
        }

        await _aavePositionRepository.UnitOfWork.SaveChangesAsync(ct);

        return result;
    }
}