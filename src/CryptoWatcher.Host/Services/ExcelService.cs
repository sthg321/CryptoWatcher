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

    public async Task<MemoryStream> ExportPoolInfoToExcelAsync(DateOnly? from, DateOnly? to)
    {
        var now = DateTime.Now;
        var monthStart = new DateTime(now.Year, now.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        if (!from.HasValue || !to.HasValue)
        {
            from = DateOnly.FromDateTime(monthStart);
            to = DateOnly.FromDateTime(monthEnd);
        }

        var pools = await _dbContext.PoolPositions
            .Include(position =>
                position.PositionFees.Where(snapshot => snapshot.Day >= from.Value && snapshot.Day <= to.Value))
            .Where(position => position.IsActive)
            .ToArrayAsync();

        var ms = new MemoryStream();
        var sheet = await Spreadsheet.CreateNewAsync(ms);

        await sheet.StartWorksheetAsync("report");

        await sheet.AddHeaderRowAsync(PoolInfoExcelRowContext.Default.PoolInfoExcel);

        var groupedPoolPositions = pools
            .OrderBy(position => position.Day)
            .GroupBy(position => new { position.PositionId, position.NetworkName })
            .ToArray();

        foreach (var groupedPoolPosition in groupedPoolPositions)
        {
            var initialPosition = groupedPoolPosition.MinBy(position => position.Day);
            foreach (var poolPosition in groupedPoolPosition)
            {
                foreach (var positionSnapshot in poolPosition.PositionFees.OrderBy(snapshot => snapshot.Day))
                {
                    await sheet.AddAsRowAsync(new PoolInfoExcel
                    {
                        Day = positionSnapshot.Day.ToShortDateString(),
                        PositionInUsd =
                            Math.Round(poolPosition.Token0.AmountInUsd + poolPosition.Token1.AmountInUsd, 2),
                        HoldInUsd = Math.Round(initialPosition!.Token0.Amount * positionSnapshot.Token0Fee.PriceInUsd +
                                               initialPosition.Token1.Amount * positionSnapshot.Token1Fee.PriceInUsd, 2),
                        TokenPairSymbol = $"{positionSnapshot.Token0Fee.Symbol} / {positionSnapshot.Token1Fee.Symbol}",
                        FeeInUsd = Math.Round(positionSnapshot.FeeInUsd, 2),
                        Network = poolPosition.NetworkName,
                    }, PoolInfoExcelRowContext.Default.PoolInfoExcel);
                }
            }

            await sheet.AddRowAsync([]);
        }
        
        var feeSum = groupedPoolPositions.Sum(groupedPoolPosition =>
        {
            var fee = groupedPoolPosition.SelectMany(position =>
                position.PositionFees.OrderBy(positionFee => positionFee.Day));

            return CalculateFeeInUsd(fee);
        });

        var positionSum = groupedPoolPositions
            .Select(grouping => grouping.MaxBy(position => position.Day))
            .Sum(position => position.Token0.AmountInUsd + position.Token1.AmountInUsd);

        var holdSum = groupedPoolPositions
            .Sum(grouping =>
            {
                var position = grouping.MinBy(poolPosition => poolPosition.Day);
                var lastPosition = grouping.MaxBy(poolPosition => poolPosition.Day);

                var fee = lastPosition!.PositionFees.First();
                
                return position!.Token0.Amount * fee.Token0Fee.PriceInUsd +
                       position.Token1.Amount * fee.Token1Fee.PriceInUsd;
            });

        await sheet.AddAsRowAsync(new PoolInfoExcel
        {
            Day = "Итого",
            PositionInUsd = Math.Round(positionSum, 2), 
            FeeInUsd = Math.Round(feeSum, 2) ,
            HoldInUsd = Math.Round(holdSum, 2),
            Network = "-",
            TokenPairSymbol = "-"
        }, PoolInfoExcelRowContext.Default.PoolInfoExcel);

        await sheet.FinishAsync();

        ms.Seek(0, SeekOrigin.Begin);

        return ms;
    }

    private decimal CalculateFeeInUsd(IEnumerable<PoolPositionFee> positionFees)
    {
        var prevDayFee = 0m;
        var result = 0m;
        foreach (var poolPositionFee in positionFees)
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
        public decimal RoiNet => Math.Round(PositionInUsd + FeeInUsd - HoldInUsd , 2); 
        
        [ColumnHeader("APY %")] public decimal Apy => Math.Round(FeeInUsd / PositionInUsd * 100 * 12, 2);

        [ColumnHeader("Пара")] public string TokenPairSymbol { get; init; } = null!;

        [ColumnHeader("Сеть")] public string Network { get; init; } = null!;
    }
}

[WorksheetRow(typeof(ExcelService.PoolInfoExcel))]
public partial class PoolInfoExcelRowContext : WorksheetRowContext;