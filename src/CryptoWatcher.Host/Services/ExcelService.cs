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
                position.PositionSnapshots.Where(snapshot => snapshot.Day >= from.Value && snapshot.Day <= to.Value))
            .Where(position => position.IsActive);

        var ms = new MemoryStream();
        var sheet = await Spreadsheet.CreateNewAsync(ms);

        await sheet.StartWorksheetAsync("report");

        await sheet.AddHeaderRowAsync(PoolInfoExcelRowContext.Default.PoolInfoExcel);
        foreach (var poolPosition in pools)
        {
            foreach (var positionSnapshot in poolPosition.PositionSnapshots.OrderBy(snapshot => snapshot.Day))
            {
                await sheet.AddAsRowAsync(new PoolInfoExcel
                {
                    Network = poolPosition.NetworkName,
                    Day = positionSnapshot.Day.ToShortDateString(),
                    TokenPairSymbol = $"{positionSnapshot.Token0Symbol} / {positionSnapshot.Token1Symbol}",
                    FeeInUsd = Math.Round(positionSnapshot.CalculateFeeInUsd(), 2)
                }, PoolInfoExcelRowContext.Default.PoolInfoExcel);
            }
        }

        await sheet.FinishAsync();

        ms.Seek(0, SeekOrigin.Begin);

        return ms;
    }

    public class PoolInfoExcel
    {
        [ColumnHeader("Сеть")] public string Network { get; init; } = null!;

        [ColumnHeader("День")] public string Day { get; init; } = null!;

        [ColumnHeader("Пара")] public string TokenPairSymbol { get; init; } = null!;

        [ColumnHeader("Комиссия в $")] public decimal FeeInUsd { get; init; }
    }
}

[WorksheetRow(typeof(ExcelService.PoolInfoExcel))] public partial class PoolInfoExcelRowContext : WorksheetRowContext;