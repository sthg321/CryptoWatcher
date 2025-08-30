using CryptoWatcher.HyperliquidModule.Services;
using SpreadCheetah;
using SpreadCheetah.SourceGeneration;

namespace CryptoWatcher.Infrastructure.Hyperliquid;

public class HyperliquidExcelService
{
    private readonly IHyperliquidReportService _hyperliquidReportService;

    public HyperliquidExcelService(IHyperliquidReportService hyperliquidReportService)
    {
        _hyperliquidReportService = hyperliquidReportService;
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

        var vaultReports = await _hyperliquidReportService.CreateReportAsync(from.Value, to.Value, ct);

        var ms = new MemoryStream();

        var sheet = await Spreadsheet.CreateNewAsync(ms, cancellationToken: ct);

        await sheet.StartWorksheetAsync("report", token: ct);

        await sheet.AddHeaderRowAsync(HyperliquidVaultPositionExcelContext.Default.HyperliquidVaultPositionExcelRow,
            token: ct);

        foreach (var vaultReport in vaultReports)
        {
            foreach (var vaultReportItem in vaultReport.ReportItems)
            {
                var row = new HyperliquidVaultPositionExcelRow
                {
                    Vault = vaultReportItem.VaultAddress,
                    Balance = Math.Round(vaultReportItem.Balance, 2),
                    Day = vaultReportItem.Day.ToString(),
                    ChangesForDay = Math.Round(vaultReportItem.DailyChange, 2),
                    ChangesForPercent = Math.Round(vaultReportItem.DailyChangePercent, 4)
                };

                await sheet.AddAsRowAsync(row,
                    HyperliquidVaultPositionExcelContext.Default.HyperliquidVaultPositionExcelRow, ct);
            }

            await sheet.AddRowAsync([
                new DataCell("Итого:"),
                new DataCell(Math.Round(vaultReport.TotalBalance, 2)),
                new DataCell("-"),
                new DataCell(Math.Round(vaultReport.TotalAbsoluteProfit, 2)),
                new DataCell(Math.Round(vaultReport.TotalPercentProfit, 4))
            ], ct);

            await sheet.AddRowAsync([], ct);
        }

        await sheet.FinishAsync(ct);
        ms.Seek(0, SeekOrigin.Begin);
        return ms;
    }
}

public class HyperliquidVaultPositionExcelRow
{
    [ColumnHeader("VaultAddress")] public string Vault { get; init; } = null!;

    [ColumnHeader("День")] public string Day { get; init; } = null!;

    [ColumnHeader("Баланс")] public decimal Balance { get; init; }

    [ColumnHeader("Изменение за день")] public decimal ChangesForDay { get; init; }

    [ColumnHeader("Изменение за день в процентах")]
    public decimal ChangesForPercent { get; init; }
}

[WorksheetRow(typeof(HyperliquidVaultPositionExcelRow))]
public partial class HyperliquidVaultPositionExcelContext : WorksheetRowContext;