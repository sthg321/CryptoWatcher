using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Extensions;
using CryptoWatcher.Models;
using CryptoWatcher.Modules.Hyperliquid.Entities;
using CryptoWatcher.Modules.Hyperliquid.Models;
using CryptoWatcher.Modules.Hyperliquid.Specifications;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Hyperliquid.Application.Services;

/// <summary>
/// <see cref="IHyperliquidPositionsSyncService"/>
/// </summary>
public class HyperliquidReportDataService : IPlatformDailyReportDataProvider
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

        var result = new Dictionary<Wallet, List<PlatformDailyReport>>();
        foreach (var vaultPositionByWallet in vaultPositions.GroupBy(position => position.WalletAddress))
        {
            foreach (var vaultPosition in vaultPositionByWallet)
            {
                var vaultReportItems = vaultPosition.PositionSnapshots.OrderBy(snapshot => snapshot.Day)
                    .Select(vaultPositionSnapshot =>
                    {
                        var previousDay = vaultPositionSnapshot.Day.AddDays(-1);
                        var profitInUsd = vaultPosition.CalculateProfitInUsd(previousDay, vaultPositionSnapshot.Day);
                        return new HyperliquidVaultReportItem
                        {
                            VaultAddress = vaultPosition.VaultAddress,
                            Day = vaultPositionSnapshot.Day,
                            PositionInUsd = vaultPositionSnapshot.Balance,
                            DailyProfitInUsd = profitInUsd.Amount,
                            DailyProfitInUsdPercent = profitInUsd.Percent
                        };
                    })
                    .ToArray();

                var totalProfit = vaultPosition.CalculateProfitInUsd(from, to);
                var vaultReport = new HyperliquidDailyReport
                {
                    PositionInUsd = vaultReportItems.Length != 0 ? vaultReportItems[^1].PositionInUsd : 0,
                    ProfitInUsd = totalProfit.Amount,
                    ProfitInPercent = totalProfit.Percent,
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