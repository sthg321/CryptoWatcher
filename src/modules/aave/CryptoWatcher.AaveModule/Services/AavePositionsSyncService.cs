using CryptoWatcher.AaveModule.Abstractions;
using CryptoWatcher.AaveModule.Entities;
using CryptoWatcher.AaveModule.Models;
using CryptoWatcher.AaveModule.Specifications;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.Shared.ValueObjects;

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
    private readonly ITokenEnricher _tokenEnricher;
    private readonly IAaveMainnetProvider _aaveMainnetProvider;
    private readonly IRepository<AavePosition> _aavePositionRepository;

    public AavePositionsSyncService(IAaveProvider aaveProvider, ITokenEnricher tokenEnricher,
        IAaveMainnetProvider aaveMainnetProvider,
        IRepository<AavePosition> aavePositionRepository)
    {
        _aaveProvider = aaveProvider;
        _tokenEnricher = tokenEnricher;
        _aaveMainnetProvider = aaveMainnetProvider;
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
                    position.TokenAddress == lendingPosition.TokenAddress &&
                    position.PositionType == lendingPosition.PositionType);

                // position is not borrowed and not supplied
                if (currentPosition is null && lendingPosition.PositionType is null)
                {
                    continue;
                }

                if (currentPosition is not null && lendingPosition.Amount == 0)
                {
                    currentPosition.ClosePosition(syncDay);
                    continue;
                }

                var tokenInfo = await FetchTokenInfoAsync(network, lendingPosition, ct);

                if (currentPosition is null)
                {
                    currentPosition = new AavePosition(network, wallet, lendingPosition.PositionType!.Value,
                        lendingPosition.TokenAddress);

                    _aavePositionRepository.Insert(currentPosition);
                }

                currentPosition.AddOrUpdateSnapshot(tokenInfo, syncDay);
            }
        }

        await _aavePositionRepository.UnitOfWork.SaveChangesAsync(ct);
    }

    private async Task<TokenInfoWithAddress> FetchTokenInfoAsync(AaveNetwork network, AaveLendingPosition position,
        CancellationToken ct = default)
    {
        var token = new Token { Address = position.TokenAddress, Balance = position.Amount };

        var mainnetAddress = _aaveMainnetProvider.GetMainnetAddressByNetworkName(network);

        return await _tokenEnricher.EnrichTokenAsync(mainnetAddress, network.Name, token, ct);
    }
}