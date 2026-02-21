using CryptoWatcher.Abstractions;
using CryptoWatcher.Modules.Morpho.Application.Models;
using CryptoWatcher.Modules.Morpho.Entities;
using CryptoWatcher.Modules.Morpho.Specifications;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Morpho.Application.Services;

public class MorphoPositionsStatusService
{
    private readonly IRepository<MorphoMarketPosition> _repository;

    public MorphoPositionsStatusService(IRepository<MorphoMarketPosition> repository)
    {
        _repository = repository;
    }

    public async Task<List<MorphoPositionsStatus>> GetPositionsStatusAsync(IReadOnlyCollection<EvmAddress> addresses,
        DateOnly day, CancellationToken ct = default)
    {
        var positions = await _repository.ListAsync(new MorphoPositionStatusSpecification(addresses, day), ct);

        var result = new List<MorphoPositionsStatus>();
        foreach (var morphoMarketPosition in positions)
        {
            var snapshot = morphoMarketPosition.Snapshots.FirstOrDefault(snapshot => snapshot.Day == day);
            if (snapshot is null)
            {
                continue;
            }

            var positionStatus = new MorphoPositionsStatus
            {
                Wallet = morphoMarketPosition.WalletAddress,
                Network = morphoMarketPosition.ChainId.ToString(),
                HealthFactor = snapshot.HealthFactor,
                Collateral = new CryptoToken
                {
                    Address = morphoMarketPosition.CollateralToken.Address,
                    Symbol = morphoMarketPosition.CollateralToken.Symbol,
                    Amount = snapshot.CollateralToken.Amount,
                    PriceInUsd = snapshot.CollateralToken.PriceInUsd,
                },
                Load = new CryptoToken
                {
                    Address = morphoMarketPosition.LoanToken.Address,
                    Symbol = morphoMarketPosition.LoanToken.Symbol,
                    Amount = snapshot.LoadToken.Amount,
                    PriceInUsd = snapshot.LoadToken.PriceInUsd,
                },
                CollateralLiquidationPrice = snapshot.CalculateCollateralPriceForLiquidation()
            };

            result.Add(positionStatus);
        }

        return result;
    }
}