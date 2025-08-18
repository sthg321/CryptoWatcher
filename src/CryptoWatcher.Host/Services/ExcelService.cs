using CryptoWatcher.Data;
using CryptoWatcher.Entities;
using Microsoft.EntityFrameworkCore;
using SpreadCheetah;
using SpreadCheetah.SourceGeneration;

namespace CryptoWatcher.Host.Services;

public class ExcelService
{
    private readonly CryptoWatcherDbContext _dbContext;

    public ExcelService(CryptoWatcherDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Stream> ExportPoolInfoToExcelAsync(DateOnly? from, DateOnly? to)
    {
        var now = DateTime.Now;
        var monthStart = new DateTime(now.Year, now.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        if (!from.HasValue || !to.HasValue)
        {
            from = DateOnly.FromDateTime(monthStart);
            to = DateOnly.FromDateTime(monthEnd);
        }

        var poolPositions = await _dbContext.PoolPositions
            .Include(position =>
                position.PoolPositionSnapshots.Where(snapshot =>
                    snapshot.Day >= from.Value && snapshot.Day <= to.Value))
            .Where(position => position.IsActive)
            .ToArrayAsync();

        var ms = new MemoryStream();
        var sheet = await Spreadsheet.CreateNewAsync(ms);

        await sheet.StartWorksheetAsync("report");

        await sheet.AddHeaderRowAsync(PoolInfoExcelRowContext.Default.PoolInfoExcel);

        foreach (var poolPosition in poolPositions)
        {
            foreach (var positionSnapshot in poolPosition.PoolPositionSnapshots.OrderBy(snapshot => snapshot.Day))
            {
                await sheet.AddAsRowAsync(new PoolInfoExcel
                {
                    Day = positionSnapshot.Day.ToShortDateString(),
                    PositionInUsd =
                        Math.Round(positionSnapshot.Token0.AmountInUsd + positionSnapshot.Token1.AmountInUsd, 2),
                    HoldInUsd = Math.Round(poolPosition.Token0.Amount * positionSnapshot.Token0.PriceInUsd +
                                           poolPosition.Token1.Amount * positionSnapshot.Token1.PriceInUsd, 2),
                    TokenPairSymbol = $"{positionSnapshot.Token0.Symbol} / {positionSnapshot.Token0.Symbol}",
                    FeeInUsd = Math.Round(positionSnapshot.FeeInUsd, 2),
                    Network = poolPosition.NetworkName,
                }, PoolInfoExcelRowContext.Default.PoolInfoExcel);
            }

            await sheet.AddRowAsync([]);
        }

        var feeSum = poolPositions.Sum(poolPosition => CalculateFeeInUsd(poolPosition.PoolPositionSnapshots));

        var positionSum = poolPositions
            .Select(poolPosition => poolPosition.PoolPositionSnapshots.MaxBy(position => position.Day))
            .Sum(snapshot => snapshot!.Token0.AmountInUsd + snapshot.Token1.AmountInUsd);

        var holdSum = poolPositions
            .Sum(poolPosition =>
            {
                var lastPosition = poolPosition.PoolPositionSnapshots.MaxBy(positionSnapshot => positionSnapshot.Day);

                return poolPosition.Token0.Amount * lastPosition!.Token0.PriceInUsd +
                       poolPosition.Token1.Amount * lastPosition.Token1.PriceInUsd;
            });

        await sheet.AddAsRowAsync(new PoolInfoExcel
        {
            Day = "Итого",
            PositionInUsd = Math.Round(positionSum, 2),
            FeeInUsd = Math.Round(feeSum, 2),
            HoldInUsd = Math.Round(holdSum, 2),
            Network = "-",
            TokenPairSymbol = "-"
        }, PoolInfoExcelRowContext.Default.PoolInfoExcel);

        await sheet.FinishAsync();

        ms.Seek(0, SeekOrigin.Begin);

        return ms;
    }

    private decimal CalculateFeeInUsd(IEnumerable<PoolPositionSnapshot> positionFees)
    {
        var prevDayFee = 0m;
        var result = 0m;
        foreach (var poolPositionFee in positionFees.OrderBy(snapshot => snapshot.Day))
        {
            if (poolPositionFee.FeeInUsd > prevDayFee)
            {
                prevDayFee = poolPositionFee.FeeInUsd;
                continue;
            }

            if (poolPositionFee.FeeInUsd < prevDayFee)
            {
                result += prevDayFee;
                prevDayFee = 0;
            }
        }

        return result + prevDayFee;
    }

    public class PoolInfoExcel
    {
        [ColumnHeader("День")] public string Day { get; init; } = null!;

        [ColumnHeader("Позиция в $")]
        [ColumnWidth(20)]
        public decimal PositionInUsd { get; init; }

        [ColumnHeader("HOLD $")]
        [ColumnWidth(20)]
        public decimal HoldInUsd { get; set; }

        [ColumnHeader("Комиссия в $")]
        [ColumnWidth(20)]
        public decimal FeeInUsd { get; init; }

        [ColumnHeader("Прибыль")]
        [ColumnWidth(20)]
        public decimal RoiNet => Math.Round(PositionInUsd + FeeInUsd - HoldInUsd, 2);

        [ColumnHeader("APY %")] public decimal Apy => Math.Round(FeeInUsd / PositionInUsd * 100 * 12, 2);

        [ColumnHeader("Пара")] public string TokenPairSymbol { get; init; } = null!;

        [ColumnHeader("Сеть")] public string Network { get; init; } = null!;
    }
}

[WorksheetRow(typeof(ExcelService.PoolInfoExcel))]
public partial class PoolInfoExcelRowContext : WorksheetRowContext;