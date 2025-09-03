using CryptoWatcher.Abstractions;
using CryptoWatcher.HyperliquidModule.Entities;
using CryptoWatcher.HyperliquidModule.Models;
using CryptoWatcher.HyperliquidModule.Specifications;

namespace CryptoWatcher.HyperliquidModule.Services;

public interface IHyperliquidReportService
{
    /// <summary>
    /// Generates a report based on the specified date range.
    /// </summary>
    /// <param name="from">The start date of the report.</param>
    /// <param name="to">The end date of the report.</param>
    /// <param name="ct">An optional cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. Returns a list of <see cref="HyperliquidVaultReport"/> objects containing the report data.</returns>
    Task<List<HyperliquidVaultReport>> CreateReportAsync(DateOnly from, DateOnly to,
        CancellationToken ct = default);
}

/// <summary>
/// <see cref="IHyperliquidPositionsSyncService"/>
/// </summary>
internal class HyperliquidReportService : IHyperliquidReportService
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
            var vaultReportItems = new List<HyperliquidVaultReportItem>(vaultPosition.PositionSnapshots.Count);
            foreach (var vaultPositionSnapshot in vaultPosition.PositionSnapshots)
            {
                var reportItem = new HyperliquidVaultReportItem
                {
                    VaultAddress = vaultPosition.VaultAddress,
                    Balance = vaultPositionSnapshot.Balance,
                    Day = vaultPositionSnapshot.Day,
                    DailyProfit = vaultPosition.CalculateAbsoluteProfit(vaultPositionSnapshot.Day.AddDays(-1),
                        vaultPositionSnapshot.Day),
                    DailyPercentProfit = vaultPosition.CalculatePercentageProfit(vaultPositionSnapshot.Day.AddDays(-1),
                        vaultPositionSnapshot.Day),
                };

                vaultReportItems.Add(reportItem);
            }

            var vaultReport = new HyperliquidVaultReport
            {
                TotalBalance = vaultReportItems.Count != 0 ? vaultReportItems[^1].Balance : 0,
                TotalAbsoluteProfit = vaultPosition.CalculateAbsoluteProfit(from, to),
                TotalPercentProfit = vaultPosition.CalculatePercentageProfit(from, to),
                ReportItems = vaultReportItems
            };

            result.Add(vaultReport);
        }

        return result;
    }
}