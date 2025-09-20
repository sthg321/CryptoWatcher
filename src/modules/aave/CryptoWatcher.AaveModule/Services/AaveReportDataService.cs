using CryptoWatcher.AaveModule.Entities;
using CryptoWatcher.AaveModule.Models;
using CryptoWatcher.AaveModule.Specifications;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Models;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.AaveModule.Services;

internal class AaveReportDataService : IPlatformDailyReportDataProvider
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
        foreach (var positionsByWallet in positions.GroupBy(position => position.WalletAddress))
        {
            foreach (var position in positionsByWallet)
            {
                var reportItems = position.PositionSnapshots.OrderBy(snapshot => snapshot.Day)
                    .Select(snapshot =>
                    {
                        var previousDay = snapshot.Day.AddDays(-1);
                        var profitInToken = position.CalculateAbsoluteProfitInToken(previousDay, snapshot.Day);
                        return new AaveDailyReportItem
                        {
                            Day = snapshot.Day,
                            TokenSymbol = snapshot.Token.Symbol,
                            PositionInUsd = snapshot.Token.AmountInUsd,
                            PositionGrowInUsd = position.CalculateAbsoluteProfitInUsd(previousDay, snapshot.Day),
                            PositionInToken = snapshot.Token.Amount,
                            DailyProfitInUsd = profitInToken * snapshot.Token.PriceInUsd,
                            DailyProfitInUsdPercent = position.CalculateAbsoluteProfitInUsd(previousDay, snapshot.Day),
                            DailyProfitInToken = position.CalculateAbsoluteProfitInToken(previousDay, snapshot.Day)
                        };
                    })
                    .ToArray();

                var lastTokenPrice = position.PositionSnapshots.LastOrDefault()?.Token.PriceInUsd ?? 0;
                var dailyReport = new AaveDailyReport
                {
                    PositionInUsd = reportItems.LastOrDefault()?.PositionInUsd ?? 0,
                    PositionInToken = reportItems.LastOrDefault()?.PositionInToken ?? 0,
                    ProfitInUsd = position.CalculateAbsoluteProfitInToken(from, to) * lastTokenPrice,
                    ProfitInPercent = 0,
                    PositionGrowInUsd = position.CalculateAbsoluteProfitInUsd(from, to),
                    ProfitInToken = position.CalculateAbsoluteProfitInToken(from, to),
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