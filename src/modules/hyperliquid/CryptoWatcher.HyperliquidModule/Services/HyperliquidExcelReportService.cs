using CryptoWatcher.Abstractions;
using CryptoWatcher.HyperliquidModule.Entities;
using CryptoWatcher.HyperliquidModule.Models;
using CryptoWatcher.HyperliquidModule.Specifications;

namespace CryptoWatcher.HyperliquidModule.Services;

public interface IHyperliquidReportService
{
    Task<List<HyperliquidVaultReport>> CreateReportAsync(DateOnly from, DateOnly to,
        CancellationToken ct = default);
}

public class HyperliquidReportService : IHyperliquidReportService
{
    private readonly IRepository<HyperliquidVaultPosition> _repository;

    public HyperliquidReportService(IRepository<HyperliquidVaultPosition> repository)
    {
        _repository = repository;
    }

    public async Task<List<HyperliquidVaultReport>> CreateReportAsync(DateOnly from, DateOnly to,
        CancellationToken ct = default)
    {
        var vaultPositions =
            await _repository.ListAsync(new HyperliquidPositionsForReportSpecification(from, to), ct);

        var result = new List<HyperliquidVaultReport>(vaultPositions.Count);
        foreach (var vaultPosition in vaultPositions)
        {
            decimal previousBalance = 0;

            var vaultReportItems = new List<HyperliquidVaultReportItem>(vaultPosition.PositionSnapshots.Count);
            foreach (var vaultPositionSnapshot in vaultPosition.PositionSnapshots)
            {
                var reportItem = new HyperliquidVaultReportItem
                {
                    VaultAddress = vaultPosition.VaultAddress,
                    Balance = Math.Round(vaultPositionSnapshot.Balance, 2),
                    Day = vaultPositionSnapshot.Day,
                    DailyProfit = previousBalance != 0 ? vaultPositionSnapshot.Balance - previousBalance : 0,
                    DailyPercentProfit = vaultPosition.CalculatePercentageChange(from, vaultPositionSnapshot.Day),
                };

                vaultReportItems.Add(reportItem);

                previousBalance = vaultPositionSnapshot.Balance;
            }

            var vaultReport = new HyperliquidVaultReport
            {
                TotalBalance = vaultReportItems.Count != 0 ? vaultReportItems[^1].Balance : 0,
                TotalAbsoluteProfit = vaultPosition.CalculateAbsoluteProfit(from, to),
                TotalPercentProfit = vaultPosition.CalculatePercentageChange(from, to),
                ReportItems = vaultReportItems
            };

            result.Add(vaultReport);
        }

        return result;
    }
}