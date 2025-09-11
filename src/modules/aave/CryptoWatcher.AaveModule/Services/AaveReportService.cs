using CryptoWatcher.AaveModule.Entities;
using CryptoWatcher.AaveModule.Models;
using CryptoWatcher.AaveModule.Specifications;
using CryptoWatcher.Abstractions;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.AaveModule.Services;

internal interface IAaveReportService
{
    Task<List<AavePositionReport>> CreateReport(Wallet wallet, DateOnly from, DateOnly to,
        CancellationToken ct = default);
}

internal class AaveReportService : IAaveReportService
{
    private readonly IRepository<AavePosition> _repository;

    public AaveReportService(IRepository<AavePosition> repository)
    {
        _repository = repository;
    }

    public async Task<List<AavePositionReport>> CreateReport(Wallet wallet, DateOnly from, DateOnly to,
        CancellationToken ct = default)
    {
        var positions =
            await _repository.ListAsync(new AavePositionsWithSnapshotsAndEventsSpecification(wallet, from, to), ct);

        var result = new List<AavePositionReport>();
        foreach (var position in positions)
        {
            var positionReport = new AavePositionReport
            {
                ReportItems = position.PositionSnapshots.OrderBy(snapshot => snapshot.Day)
                    .Select(snapshot => new AavePositionReportItem
                    {
                        Day = snapshot.Day,
                        Token = snapshot.Token,
                        DailyProfitInUsd = position.CalculateAbsoluteProfitInUsd(from, snapshot.Day),
                        DailyPercentProfitInUsd = position.CalculatePercentageProfit(from, snapshot.Day),
                        DailyTokenInProfit = position.CalculateAbsoluteProfitInToken(from, snapshot.Day)
                    })
                    .ToArray()
            };

            result.Add(positionReport);
        }

        return result;
    }
}