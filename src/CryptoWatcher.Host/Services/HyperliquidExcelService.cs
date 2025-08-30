using CryptoWatcher.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SpreadCheetah;
using SpreadCheetah.SourceGeneration;

namespace CryptoWatcher.Host.Services;

public class HyperliquidExcelService
{
    private readonly CryptoWatcherDbContext _context;

    public HyperliquidExcelService(CryptoWatcherDbContext context)
    {
        _context = context;
    }

    public async Task<Stream> CreateReportAsync(DateOnly? from, DateOnly? to, CancellationToken ct = default)
    {
        var now = DateTime.Now;
        var monthStart = new DateTime(now.Year, now.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        if (!from.HasValue || !to.HasValue)
        {
            from = DateOnly.FromDateTime(monthStart);
            to = DateOnly.FromDateTime(monthEnd);
        }

        var vaultPositions = await _context.HyperliquidVaultPositions
            .Include(position => position.VaultEvents)
            .Include(position =>
                position.PositionSnapshots
                    .OrderBy(snapshot => snapshot.Day)
                    .Where(snapshot => snapshot.Day >= from.Value && snapshot.Day <= to.Value))
            .ToArrayAsync(ct);

        var ms = new MemoryStream();

        var sheet = await Spreadsheet.CreateNewAsync(ms, cancellationToken: ct);

        await sheet.StartWorksheetAsync("report", token: ct);

        await sheet.AddHeaderRowAsync(HyperliquidVaultPositionExcelContext.Default.HyperliquidVaultPositionExcelRow,
            token: ct);

        foreach (var vaultPosition in vaultPositions)
        {
            decimal previousBalance = 0;
            foreach (var vaultPositionSnapshot in vaultPosition.PositionSnapshots)
            {
                var row = new HyperliquidVaultPositionExcelRow
                {
                    Vault = vaultPosition.VaultAddress,
                    Balance = Math.Round(vaultPositionSnapshot.Balance, 2),
                    Day = vaultPositionSnapshot.Day.ToString(),
                    ChangesForDay =
                        Math.Round(previousBalance != 0 ? vaultPositionSnapshot.Balance - previousBalance : 0, 2),
                    ChangesForPercent =
                        Math.Round(vaultPosition.CalculatePercentageChange(from.Value, vaultPositionSnapshot.Day), 4)
                };

                await sheet.AddAsRowAsync(row,
                    HyperliquidVaultPositionExcelContext.Default.HyperliquidVaultPositionExcelRow, ct);

                previousBalance = vaultPositionSnapshot.Balance;
            }

            await sheet.AddRowAsync([], ct);

            await sheet.AddRowAsync([
                new DataCell("Absolute profit"),
                new DataCell(vaultPosition.CalculateAbsoluteProfit(from.Value, to.Value))
            ], ct);

            await sheet.FinishAsync(ct);
        }

        ms.Seek(0, SeekOrigin.Begin);
        return ms;
    }
}

public class HyperliquidVaultPositionExcelRow
{
    [ColumnHeader("Vault")] public string Vault { get; init; } = null!;

    [ColumnHeader("День")] public string Day { get; init; } = null!;

    [ColumnHeader("Баланс")] public decimal Balance { get; init; }

    [ColumnHeader("Изменение за день")] public decimal ChangesForDay { get; init; }

    [ColumnHeader("Изменение за день в процентах")]
    public decimal ChangesForPercent { get; init; }
}

[WorksheetRow(typeof(HyperliquidVaultPositionExcelRow))]
public partial class HyperliquidVaultPositionExcelContext : WorksheetRowContext;