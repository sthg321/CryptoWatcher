using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Aave.Application.Abstractions;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.Modules.Aave.Specifications;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Aave.Application.Services;

public class AaveAccountStatusService
{
    private readonly IRepository<AavePosition> _repository;
    private readonly IAaveHealthFactorCalculator _aaveHealthFactorCalculator;

    public AaveAccountStatusService(IRepository<AavePosition> repository, IAaveHealthFactorCalculator aaveHealthFactorCalculator)
    {
        _repository = repository;
        _aaveHealthFactorCalculator = aaveHealthFactorCalculator;
    }

    public async Task<double> Test(IReadOnlyCollection<EvmAddress> wallets, DateOnly day, CancellationToken ct = default)
    {
        var positions = await _repository.ListAsync(new AavePositionsWithSnapshotsSpecification(wallets, day), ct);

        var totalDebt = positions.Where(position => position.PositionType == AavePositionType.Borrowed)
            .SelectMany(position => position.Snapshots)
            .Where(snapshot => snapshot.Day == day)
            .Sum(snapshot => snapshot.Token0.AmountInUsd);

        var liquidationCalculator = new AaveLiquidationCalculator();

        var supplySnapshots = positions.Where(position => position.PositionType == AavePositionType.Supplied)
            .SelectMany(position => position.Snapshots)
            .Where(snapshot => snapshot.Day == day)
            .ToArray();

        var result = liquidationCalculator.AccountLiquidationValue(totalDebt, supplySnapshots);
        var hf = _aaveHealthFactorCalculator.CalculateHealthFactor(positions);
        
        return hf;
    }
}