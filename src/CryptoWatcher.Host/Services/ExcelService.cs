using CryptoWatcher.Data;
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

        var pools = _dbContext.LiquidityPoolPositions
            .Include(position =>
                position.PositionFees.Where(snapshot => snapshot.Day >= from.Value && snapshot.Day <= to.Value))
            .Where(position => position.IsActive);

        var ms = new MemoryStream();
        var sheet = await Spreadsheet.CreateNewAsync(ms);

        await sheet.StartWorksheetAsync("report");

        await sheet.AddHeaderRowAsync(PoolInfoExcelRowContext.Default.PoolInfoExcel);
        foreach (var poolPosition in pools)
        {
            foreach (var positionSnapshot in poolPosition.PositionFees.OrderBy(snapshot => snapshot.Day))
            {
                await sheet.AddAsRowAsync(new PoolInfoExcel
                {
                    Day = positionSnapshot.Day.ToShortDateString(),
                    PositionInUsd = Math.Round(poolPosition.Token0.AmountInUsd + poolPosition.Token1.AmountInUsd, 2),
                    TokenPairSymbol = $"{positionSnapshot.Token0Fee.Symbol} / {positionSnapshot.Token1Fee.Symbol}",
                    FeeInUsd = Math.Round(positionSnapshot.CalculateFeeInUsd(), 2),
                    Network = poolPosition.NetworkName,
                }, PoolInfoExcelRowContext.Default.PoolInfoExcel);
            }

            await sheet.AddRowAsync([]);
        }

        await sheet.FinishAsync();

        ms.Seek(0, SeekOrigin.Begin);

        return ms;
    }

    public class PoolInfoExcel
    {
        [ColumnHeader("Комиссия в $")]
        [ColumnWidth(255)]
        public decimal FeeInUsd { get; init; }

        [ColumnHeader("Позиция в $")]
        [ColumnWidth(255)]
        public decimal PositionInUsd { get; init; }

        [ColumnHeader("APY %")] public decimal Apy => Math.Round(FeeInUsd / PositionInUsd * 100 * 12, 2);
        
        [ColumnHeader("День")]
        public string Day { get; init; } = null!;

        [ColumnHeader("Пара")]
        public string TokenPairSymbol { get; init; } = null!;

        [ColumnHeader("Сеть")]
        public string Network { get; init; } = null!;
        
        private decimal GetAnnualizationFactor()
        {
            var daysActive = (DateTime.Now - DateTime.Parse(Day)).TotalDays;
            if (daysActive <= 0) return 12;
        
            return 365m / (decimal)daysActive;
        }
    }
}

[WorksheetRow(typeof(ExcelService.PoolInfoExcel))]
public partial class PoolInfoExcelRowContext : WorksheetRowContext;