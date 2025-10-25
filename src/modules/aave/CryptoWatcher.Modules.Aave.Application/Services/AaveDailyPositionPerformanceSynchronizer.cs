using CryptoWatcher.Abstractions;
using CryptoWatcher.Application.Abstractions;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.Modules.Aave.Specifications;
using CryptoWatcher.Shared.Entities;
using Microsoft.Extensions.Logging;

namespace CryptoWatcher.Modules.Aave.Application.Services;

public class AaveDailyPositionPerformanceSynchronizer :
    BaseDailyPositionPerformanceSynchronizer<AaveDailyBalanceChange>,
    IDailyPositionPerformanceSynchronizer
{
    private readonly IRepository<AavePosition> _positionRepository;

    public AaveDailyPositionPerformanceSynchronizer(IRepository<AaveDailyBalanceChange> balanceChangeRepository,
        ILogger<AaveDailyPositionPerformanceSynchronizer> logger,
        IRepository<AavePosition> positionRepository) : base(balanceChangeRepository, logger)
    {
        _positionRepository = positionRepository;
    }

    public string Name => "Aave";

    protected override async Task<List<AaveDailyBalanceChange>> GetDailyBalanceChangesAsync(
        IReadOnlyCollection<Wallet> wallets, DateOnly from, DateOnly to, CancellationToken ct)
    {
        var result = new List<AaveDailyBalanceChange>();

        var positions =
            await _positionRepository.ListAsync(new AavePositionsWithSnapshotsAndEventsSpecification(wallets, from, to),
                ct);

        foreach (var aavePosition in positions)
        {
            AavePositionSnapshot? previousSnapshot = null;
            foreach (var currentSnapshot in aavePosition.PositionSnapshots)
            {
                previousSnapshot ??= currentSnapshot;

                var balanceChange = AaveDailyBalanceChange.Create(aavePosition, previousSnapshot, currentSnapshot);

                result.Add(balanceChange);
            }
        }

        return result;
    }
}