using CryptoWatcher.AaveModule.Abstractions;
using CryptoWatcher.AaveModule.Entities;
using CryptoWatcher.AaveModule.Models;
using CryptoWatcher.AaveModule.Specifications;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Shared.Entities;

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
    /// <param name="wallet">The wallet entity containing the address to fetch lending positions for.</param>
    /// <param name="syncDay"></param>
    /// <param name="ct">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SyncPositionsAsync(Wallet wallet, DateOnly syncDay, CancellationToken ct = default);
}

internal class AavePositionsSyncService : IAavePositionsSyncService
{
    private readonly IAaveProvider _aaveProvider;
    private readonly IRepository<AavePosition> _aavePositionRepository;

    public AavePositionsSyncService(IAaveProvider aaveProvider, IRepository<AavePosition> aavePositionRepository)
    {
        _aaveProvider = aaveProvider;
        _aavePositionRepository = aavePositionRepository;
    }

    public async Task SyncPositionsAsync(Wallet wallet, DateOnly syncDay, CancellationToken ct = default)
    {
        var existedPositions = await _aavePositionRepository.ListAsync(
                new AavePositionsWithSnapshotsSpecification(wallet.Address, syncDay, syncDay), ct);
 
        foreach (var network in AaveNetwork.All)
        {
            var lendingPositions = await _aaveProvider.GetLendingPositionAsync(network, wallet, ct);

            foreach (var lendingPosition in lendingPositions)
            {
                var currentPosition = existedPositions.FirstOrDefault(position =>
                    position.TokenAddress == lendingPosition.Token.Address &&
                    position.PositionType == lendingPosition.PositionType);

                switch (currentPosition)
                {
                    // position is not borrowed and not supplied
                    case null when lendingPosition.Token.Amount == 0:
                        continue;
                    case null:
                        currentPosition = new AavePosition(network, wallet, lendingPosition.PositionType,
                            lendingPosition.Token.Address);

                        _aavePositionRepository.Insert(currentPosition);
                        currentPosition.AddOrUpdateSnapshot(lendingPosition.Token, syncDay);
                        continue;
                }

                if (lendingPosition.Token.Amount == 0)
                {
                    currentPosition.ClosePosition(syncDay);
                }
            }
        }

        await _aavePositionRepository.UnitOfWork.SaveChangesAsync(ct);
    }
}