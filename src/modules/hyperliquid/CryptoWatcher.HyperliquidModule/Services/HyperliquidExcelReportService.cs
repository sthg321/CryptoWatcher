using CryptoWatcher.Abstractions;
using CryptoWatcher.HyperliquidModule.Entities;
using CryptoWatcher.HyperliquidModule.Models;
using CryptoWatcher.HyperliquidModule.Specifications;
using CryptoWatcher.Models;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.HyperliquidModule.Services;

/// <summary>
/// <see cref="IHyperliquidPositionsSyncService"/>
/// </summary>
internal class HyperliquidReportDataService : IPlatformDailyReportDataProvider
{
    private readonly IRepository<HyperliquidVaultPosition> _repository;

    public HyperliquidReportDataService(IRepository<HyperliquidVaultPosition> repository)
    {
        _repository = repository;
    }

    public async Task<PlatformDailyReportData> GetReportDataAsync(IReadOnlyCollection<Wallet> wallets,
        DateOnly from, DateOnly to, CancellationToken ct = default)
    {
        var vaultPositions =
            await _repository.ListAsync(new HyperliquidPositionsForReportSpecification(wallets, from, to), ct);

        var result = new Dictionary<Wallet, List<PlatformDailyReport>>(vaultPositions.Count);
        foreach (var vaultPositionByWallet in vaultPositions.GroupBy(position => position.WalletAddress))
        {
            foreach (var vaultPosition in vaultPositionByWallet)
            {
                var vaultReportItems = new List<HyperliquidVaultReportItem>(vaultPosition.PositionSnapshots.Count);
                foreach (var vaultPositionSnapshot in vaultPosition.PositionSnapshots)
                {
                    var previousDay = vaultPositionSnapshot.Day.AddDays(-1);
                    var reportItem = new HyperliquidVaultReportItem
                    {
                        VaultAddress = vaultPosition.VaultAddress,
                        Balance = vaultPositionSnapshot.Balance,
                        Day = vaultPositionSnapshot.Day,
                        DailyProfit = vaultPosition.CalculateAbsoluteProfit(previousDay,
                            vaultPositionSnapshot.Day),
                        DailyPercentProfit = vaultPosition.CalculatePercentageProfit(previousDay,
                            vaultPositionSnapshot.Day),
                    };

                    vaultReportItems.Add(reportItem);
                }

                var vaultReport = new HyperliquidVaultReport
                {
                    PositionInUsd = vaultReportItems.Count != 0 ? vaultReportItems[^1].Balance : 0,
                    ProfitInUsd = vaultPosition.CalculateAbsoluteProfit(from, to),
                    ProfitInPercent = vaultPosition.CalculatePercentageProfit(from, to),
                    ReportItems = vaultReportItems
                };

                if (!result.TryGetValue(vaultPosition.Wallet, out var dailyReports))
                {
                    result.Add(vaultPosition.Wallet, dailyReports = []);
                }

                dailyReports.Add(vaultReport);
            }
        }

        return new PlatformDailyReportData
        {
            PlatformName = "Hyperliquid",
            Reports = result
        };
    }
}