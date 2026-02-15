using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Extensions;
using CryptoWatcher.Models;
using CryptoWatcher.Modules.Hyperliquid.Application.Features.Reports.Abstractions;
using CryptoWatcher.Modules.Hyperliquid.Application.Features.Reports.Models;
using CryptoWatcher.Shared.Entities;
using CryptoWatcher.ValueObjects;

namespace CryptoWatcher.Modules.Hyperliquid.Application.Features.Reports;
 
public class HyperliquidReportDataService : IPlatformDailyReportDataProvider
{
    private readonly IHyperliquidPositionForReportQuery _query;

    public HyperliquidReportDataService(IHyperliquidPositionForReportQuery query)
    {
        _query = query;
    }

    public async Task<PlatformDailyReportData> GetReportDataAsync(IReadOnlyCollection<Wallet> wallets,
        DateOnly from, DateOnly to, CancellationToken ct = default)
    {
        var walletAddresses = wallets.Select(wallet => wallet.Address).ToArray();
        var vaultPositions = await _query.GetPositionsAsync(walletAddresses, from, to, ct);

        var result = new Dictionary<EvmAddress, List<PlatformDailyReport>>();
        foreach (var vaultPositionByWallet in vaultPositions.GroupBy(position => position.WalletAddress))
        {
            foreach (var vaultPosition in vaultPositionByWallet)
            {
                var vaultReportItems = vaultPosition.Snapshots.OrderBy(snapshot => snapshot.Day)
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

                if (!result.TryGetValue(vaultPosition.WalletAddress, out var dailyReports))
                {
                    result.Add(vaultPosition.WalletAddress, dailyReports = []);
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