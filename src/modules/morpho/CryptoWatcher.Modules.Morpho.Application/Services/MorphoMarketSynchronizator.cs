using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Morpho.Application.Abstractions;
using CryptoWatcher.Modules.Morpho.Entities;
using CryptoWatcher.Modules.Morpho.Specifications;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Morpho.Application.Services;

public class MorphoMarketSynchronizer : IMorphoMarketSynchronizer
{
    private readonly IMorphoProvider _morphoProvider;
    private readonly IRepository<MorphoMarketPosition> _morphoMarketPositionRepository;
    private readonly IRepository<MorphoMarketPositionSnapshot> _morphoMarketPositionSnapshotRepository;

    public MorphoMarketSynchronizer(IMorphoProvider morphoProvider,
        IRepository<MorphoMarketPosition> morphoMarketPositionRepository,
        IRepository<MorphoMarketPositionSnapshot> morphoMarketPositionSnapshotRepository)
    {
        _morphoProvider = morphoProvider;
        _morphoMarketPositionRepository = morphoMarketPositionRepository;
        _morphoMarketPositionSnapshotRepository = morphoMarketPositionSnapshotRepository;
    }

    public async Task SynchronizeAsync(EvmAddress walletAddress, int chainId, DateTime syncDate, CancellationToken ct)
    {
        var syncDay = DateOnly.FromDateTime(syncDate);

        var marketPositions = await _morphoProvider.GetUserMarketPositionsAsync(walletAddress, chainId, ct);
        if (marketPositions.Length == 0)
        {
            return;
        }

        var dbMarketPositions = (await _morphoMarketPositionRepository.ListAsync(
                new MorphoMarketActivePositions(walletAddress, syncDay, syncDay), ct))
            .ToDictionary(position => position.MarketExternalId);

        var result = new List<MorphoMarketPosition>();

        foreach (var marketPosition in marketPositions)
        {
            if (!dbMarketPositions.TryGetValue(marketPosition.MarketId, out var dbMarketPosition))
            {
                if (marketPosition.IsEmpty())
                {
                    continue;
                }

                dbMarketPosition = new MorphoMarketPosition(walletAddress, marketPosition.MarketId, chainId,
                    marketPosition.LoanToken, marketPosition.CollateralToken, syncDate);
            }

            if (marketPosition.IsEmpty())
            {
                dbMarketPosition.ClosePosition(syncDate);
                result.Add(dbMarketPosition);
                continue;
            }

            dbMarketPosition.AddSnapshot(syncDay, marketPosition.LoanToken.ToStatistic(),
                marketPosition.CollateralToken.ToStatistic(), marketPosition.HealthFactor);

            result.Add(dbMarketPosition);
        }

        await _morphoMarketPositionRepository.UnitOfWork.BeginTransactionAsync(ct);
        
        await _morphoMarketPositionRepository.BulkMergeAsync(result, ct);

        var snapshots = result.SelectMany(position => position.Snapshots).ToArray();

        await _morphoMarketPositionSnapshotRepository.BulkMergeAsync(snapshots, ct);

        await _morphoMarketPositionRepository.UnitOfWork.CommitTransactionAsync(ct);
    }
}