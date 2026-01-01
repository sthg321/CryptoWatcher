using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Morpho.Application.Abstractions;
using CryptoWatcher.Modules.Morpho.Entities;
using CryptoWatcher.Modules.Morpho.Specifications;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Morpho.Application.Services;

public class MorphoMarketSynchronizator
{
    private readonly IMorphoProvider _morphoProvider;
    private readonly IRepository<MorphoMarketPosition> _morphoMarketPositionRepository;
    private readonly TimeProvider _timeProvider;

    public MorphoMarketSynchronizator(IMorphoProvider morphoProvider,
        IRepository<MorphoMarketPosition> morphoMarketPositionRepository, TimeProvider timeProvider)
    {
        _morphoProvider = morphoProvider;
        _morphoMarketPositionRepository = morphoMarketPositionRepository;
        _timeProvider = timeProvider;
    }

    public async Task SynchronizeAsync(EvmAddress walletAddress, int chainId, CancellationToken ct)
    {
        var marketPositions = await _morphoProvider.GetUserMarketPositionsAsync(walletAddress, chainId, ct);

        var dbMarketPositions =
            (await _morphoMarketPositionRepository.ListAsync(new MorphoMarketActivePositions(walletAddress), ct))
            .ToDictionary(position => position.MarketExternalId);

        var result = new List<MorphoMarketPosition>();

        var today = _timeProvider.GetUtcNow().UtcDateTime;

        foreach (var marketPosition in marketPositions)
        {
            if (!dbMarketPositions.TryGetValue(marketPosition.MarketId, out var dbMarketPosition))
            {
                dbMarketPosition = new MorphoMarketPosition(walletAddress, marketPosition.MarketId, chainId,
                    marketPosition.LoanToken, marketPosition.CollateralToken, today);
                result.Add(dbMarketPosition);
                continue;
            }

            if (marketPosition.LoanToken.Amount == 0 && marketPosition.CollateralToken.Amount == 0)
            {
                dbMarketPosition.ClosePosition(today);
                result.Add(dbMarketPosition);
                continue;
            }

            dbMarketPosition.AddSnapshot(DateOnly.FromDateTime(today), marketPosition.LoanToken.ToStatistic(),
                marketPosition.CollateralToken.ToStatistic(), marketPosition.HealthFactor);

            result.Add(dbMarketPosition);
        }

        await _morphoMarketPositionRepository.BulkMergeAsync(result, ct);
    }
}