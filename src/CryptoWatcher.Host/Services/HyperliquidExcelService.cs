using CryptoWatcher.Data;
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
            .Include(position =>
                position.PositionSnapshots
                    .OrderBy(snapshot => snapshot.Day)
                    .Where(snapshot => snapshot.Day >= from.Value && snapshot.Day <= to.Value))
            .ToArrayAsync(ct);

        var ms = new MemoryStream();

        var sheet = await Spreadsheet.CreateNewAsync(ms, cancellationToken: ct);

        await sheet.StartWorksheetAsync("report", token: ct);

        foreach (var vaultPosition in vaultPositions)
        {
            decimal previousBalance = 0;
            foreach (var vaultPositionSnapshot in vaultPosition.PositionSnapshots)
            {
                var row = new HyperliquidVaultPositionExcelRow
                {
                    Vault = vaultPosition.VaultAddress,
                    Balance = vaultPositionSnapshot.Balance,
                    Day = vaultPositionSnapshot.Day.ToString(),
                    Profit = vaultPositionSnapshot.Balance - previousBalance
                };

                await sheet.AddAsRowAsync(row,
                    HyperliquidVaultPositionExcelContext.Default.HyperliquidVaultPositionExcelRow, ct);
                
                previousBalance = vaultPositionSnapshot.Balance;
            }

            await sheet.FinishAsync(ct);
        }

        ms.Seek(0, SeekOrigin.Begin);
        return ms;
    }
}

public class HyperliquidVaultPositionExcelRow
{
    public string Vault { get; set; } = null!;

    public string Day { get; set; } = null!;

    public decimal Balance { get; set; }

    public decimal Profit { get; set; }
}

[WorksheetRow(typeof(HyperliquidVaultPositionExcelRow))]
public partial class HyperliquidVaultPositionExcelContext : WorksheetRowContext;