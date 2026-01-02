using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Aave.Application.Abstractions;
using CryptoWatcher.Modules.Aave.Application.Models;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.Modules.Aave.Models;
using CryptoWatcher.Modules.Aave.Specifications;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CryptoWatcher.Modules.Aave.Application.Services;

public class AavePositionsSyncService : IAavePositionsSyncService
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
        AaveChainConfiguration chain,
        Wallet wallet,
        DateOnly syncDay,
        CancellationToken ct = default)
    {
        var existedPositions = await _aavePositionRepository.ListAsync(
            new AavePositionsWithSnapshotsSpecification(chain, wallet, syncDay, syncDay),
            ct); // Предполагаем, что spec грузит открытые позиции; если нет — обновите spec на all open.

        _logger.LogExistedPositionsForWalletCount(wallet.Address, existedPositions.Count);

        var result = new List<AavePosition>();

        var aavePositionsResponse = await _aaveProvider.GetLendingPositionAsync(chain, wallet);

        _logger.LogFetchedPositionsForNetworkCount(chain.Name, aavePositionsResponse.Positions.Count);

        foreach (var lendingPosition in aavePositionsResponse.Positions)
        {
            if (lendingPosition is EmptyAaveLendingPosition)
            {
                foreach (var position in existedPositions.Where(position =>
                             position.Token0.Address.Equals(lendingPosition.TokenAddress)))
                {
                    position.ClosePosition(syncDay);
                    result.Add(position);

                    _aavePositionRepository.Update(position);
                    _logger.LogPositionClosed(position.Id, position.Token0.Address);
                }

                continue;
            }
            
            var calculatableAaveLendingPosition = lendingPosition as CalculatableAaveLendingPosition ??
                                                  throw new InvalidOperationException("...");

            var cryptoToken = await _aaveTokenEnricher.EnrichTokenAsync(chain, calculatableAaveLendingPosition, ct);
            
            var positionType = calculatableAaveLendingPosition.DeterminePositionType();

            var currentPosition = existedPositions.FirstOrDefault(position =>
                position.Token0.Address.Equals(cryptoToken.Address) && position.PositionType == positionType);

            if (currentPosition is null)
            {
                currentPosition = new AavePosition(chain, wallet, positionType, cryptoToken, syncDay);

                _aavePositionRepository.Insert(currentPosition);

                _logger.LogCreateAavePosition(currentPosition.Token0.Address, cryptoToken);
            }
            else
            {
                _aavePositionRepository.Update(currentPosition);

                _logger.LogUpdateAavePosition(currentPosition.Token0.Address, cryptoToken);
            }

            var positionScaleAmount = calculatableAaveLendingPosition.CalculatePositionScaleInToken();
            currentPosition.AddOrUpdateSnapshot(cryptoToken, positionScaleAmount, syncDay, _timeProvider,
                aavePositionsResponse.HealthFactor);

            result.Add(currentPosition);
        }

        await _aavePositionRepository.UnitOfWork.SaveChangesAsync(ct);

        return result;
    }
}