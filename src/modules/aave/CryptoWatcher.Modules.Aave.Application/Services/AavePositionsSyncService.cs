using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Aave.Application.Abstractions;
using CryptoWatcher.Modules.Aave.Application.Models;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.Shared.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CryptoWatcher.Modules.Aave.Application.Services;

public class AavePositionsSyncService : IAavePositionsSyncService
{
    private readonly IAaveProvider _aaveProvider;
    private readonly IAaveTokenEnricher _aaveTokenEnricher;
    private readonly IAavePositionRepository _aavePositionRepository;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<AavePositionsSyncService> _logger;

    public AavePositionsSyncService(IAaveProvider aaveProvider, IAaveTokenEnricher aaveTokenEnricher,
        IAavePositionRepository aavePositionRepository, TimeProvider timeProvider,
        IRepository<AaveAccountSnapshot> aaveAccountSnapshotRepository,
        ILogger<AavePositionsSyncService>? logger = null)
    {
        _aaveProvider = aaveProvider;
        _aaveTokenEnricher = aaveTokenEnricher;
        _aavePositionRepository = aavePositionRepository;
        _timeProvider = timeProvider;
        _logger = logger ?? NullLogger<AavePositionsSyncService>.Instance;
    }

    public async Task<List<AavePosition>> SyncPositionsAsync(
        AaveProtocolConfiguration protocol,
        Wallet wallet,
        DateOnly syncDay,
        CancellationToken ct = default)
    {
        var existedPositions =
            await _aavePositionRepository.GetActiveForWalletAsync(protocol.Name, wallet.Address, syncDay, ct);

        _logger.LogExistedPositionsForWalletCount(wallet.Address, existedPositions.Count);

        var result = new List<AavePosition>();

        var aavePositionsResponse = await _aaveProvider.GetLendingPositionAsync(protocol, wallet);

        _logger.LogFetchedPositionsForNetworkCount(protocol.Name, aavePositionsResponse.Positions.Count);

       
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

            var cryptoToken = await _aaveTokenEnricher.EnrichTokenAsync(protocol, lendingPosition, ct);

            var currentPosition = existedPositions.FirstOrDefault(position =>
                position.Token0.Address.Equals(cryptoToken.Address) &&
                position.PositionType == lendingPosition.PositionType);

            if (currentPosition is null)
            {
                currentPosition = new AavePosition(protocol, wallet.Address, lendingPosition.PositionType, cryptoToken,
                    syncDay);

                _aavePositionRepository.Add(currentPosition);

                _logger.LogCreateAavePosition(currentPosition.Token0.Address, cryptoToken);
            }
            else
            {
                _aavePositionRepository.Update(currentPosition);

                _logger.LogUpdateAavePosition(currentPosition.Token0.Address, cryptoToken);
            }

            var positionScaleAmount = lendingPosition.PrincipalAmount;
            currentPosition.AddOrUpdateSnapshot(cryptoToken, positionScaleAmount, syncDay, _timeProvider,
                (double?)lendingPosition.LiquidationLtv);

            result.Add(currentPosition);
        }

        await _aavePositionRepository.SaveAsync(result, ct);

        return result;
    }
}