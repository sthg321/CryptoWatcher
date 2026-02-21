using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Aave.Application.Abstractions;
using CryptoWatcher.Modules.Aave.Application.Models;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.Modules.Aave.Models;
using CryptoWatcher.Modules.Aave.Specifications;
using CryptoWatcher.Shared.Entities;
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
    private readonly IRepository<AaveAccountSnapshot>  _aaveAccountSnapshotRepository;

    public AavePositionsSyncService(IAaveProvider aaveProvider, IAaveTokenEnricher aaveTokenEnricher,
        IRepository<AavePosition> aavePositionRepository, TimeProvider timeProvider, IRepository<AaveAccountSnapshot> aaveAccountSnapshotRepository, ILogger<AavePositionsSyncService>? logger = null)
    {
        _aaveProvider = aaveProvider;
        _aaveTokenEnricher = aaveTokenEnricher;
        _aavePositionRepository = aavePositionRepository;
        _timeProvider = timeProvider;
        _aaveAccountSnapshotRepository = aaveAccountSnapshotRepository;
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
            ct);

        _logger.LogExistedPositionsForWalletCount(wallet.Address, existedPositions.Count);

        var result = new List<AavePosition>();

        var aavePositionsResponse = await _aaveProvider.GetLendingPositionAsync(chain, wallet);

        _logger.LogFetchedPositionsForNetworkCount(chain.Name, aavePositionsResponse.Positions.Count);

        var totalSupply = 0M;
        var totalBorrow = 0M;
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
                (double?)(calculatableAaveLendingPosition as SuppliedAaveLendingPosition)?.LiquidationLtv);

            result.Add(currentPosition);

            if (positionType == AavePositionType.Supplied)
            {
                totalSupply += cryptoToken.AmountInUsd;
                continue;
            }

            totalBorrow += cryptoToken.AmountInUsd;
        }

        var accountSnapshot = new AaveAccountSnapshot
        {
            HealthFactor = aavePositionsResponse.HealthFactor,
            Day = syncDay,
            WalletAddress = wallet.Address,
            NetworkName = chain.Name,
            TotalCollateralInUsd = totalSupply,
            TotalDebtInUsd = totalBorrow
        };
        
        await _aavePositionRepository.UnitOfWork.SaveChangesAsync(ct);

        await _aaveAccountSnapshotRepository.BulkMergeAsync([accountSnapshot], ct);
        
        return result;
    }
}