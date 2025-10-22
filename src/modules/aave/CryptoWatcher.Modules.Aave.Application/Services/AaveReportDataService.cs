using System.Security.Cryptography;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Abstractions.Reports;
using CryptoWatcher.Extensions;
using CryptoWatcher.Models;
using CryptoWatcher.Modules.Aave.Entities;
using CryptoWatcher.Modules.Aave.Models;
using CryptoWatcher.Modules.Aave.Specifications;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Modules.Aave.Application.Services;

public class AaveReportDataService : IPlatformDailyReportDataProvider
{
    private readonly IRepository<AavePosition> _repository;

    public AaveReportDataService(IRepository<AavePosition> repository)
    {
        _repository = repository;
    }

    public async Task<PlatformDailyReportData> GetReportDataAsync(IReadOnlyCollection<Wallet> wallets,
        DateOnly from,
        DateOnly to, CancellationToken ct = default)
    {
        var positions =
            await _repository.ListAsync(new AavePositionsWithSnapshotsAndEventsSpecification(wallets, from, to), ct);

        var result = new Dictionary<Wallet, List<PlatformDailyReport>>();
        foreach (var positionsByWallet in positions.GroupBy(static position => position.WalletAddress))
        {
            foreach (var position in positionsByWallet)
            {
                var sign = position.PositionType == AavePositionType.Borrowed ? -1 : 1;
                var reportItems = position.PositionSnapshots.OrderBy(static snapshot => snapshot.Day)
                    .Select(snapshot =>
                    {
                        var previousDay = snapshot.Day.AddDays(-1);
                        var profitInUsd = position.CalculateProfitInUsd(previousDay, snapshot.Day);
                        var profitInToken = position.CalculateProfitInToken(previousDay, snapshot.Day);

                        return new AaveDailyReportItem
                        {
                            Day = snapshot.Day,
                            TokenSymbol = snapshot.Token.Symbol,
                            PositionInUsd = snapshot.Token.AmountInUsd * sign,
                            PositionGrowInUsd = profitInUsd.Amount * sign,
                            
                            PositionInToken = snapshot.Token.Amount * sign,
                            DailyProfitInUsd = profitInToken.Amount * snapshot.Token.PriceInUsd * sign,
                            DailyProfitInUsdPercent = profitInUsd.Percent * sign,
                            DailyProfitInToken = profitInToken.Amount * sign
                        };
                    })
                    .ToArray();

                var lastTokenPrice = position.PositionSnapshots.LastOrDefault()?.Token.PriceInUsd ?? 0;
                var profitInUsd = position.CalculateProfitInUsd(from, to);

                var profitInToken = position.CalculateProfitInToken(from, to);
                var dailyReport = new AaveDailyReport
                {
                    PositionInUsd = reportItems.LastOrDefault()?.PositionInUsd ?? 0 * sign,
                    PositionInToken = reportItems.LastOrDefault()?.PositionInToken ?? 0 * sign,
                    ProfitInUsd = profitInToken.Amount * lastTokenPrice * sign,
                    ProfitInPercent = profitInUsd.Percent * sign,
                    PositionGrowInUsd = profitInUsd.Amount * sign,
                    ProfitInToken = profitInToken.Amount * sign,
                    ReportItems = reportItems
                };

                if (!result.TryGetValue(position.Wallet, out var dailyReports))
                {
                    dailyReports = [];
                    result.Add(position.Wallet, dailyReports);
                }

                dailyReports.Add(dailyReport);
            }
        }

        return new PlatformDailyReportData
        {
            PlatformName = "Aave",
            Reports = result
        };
    }
}