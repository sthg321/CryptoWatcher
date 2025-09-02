using CryptoWatcher.Extensions;
using CryptoWatcher.HyperliquidModule.Services;
using CryptoWatcher.Infrastructure.Excel;
using CryptoWatcher.Infrastructure.Hyperliquid.ExcelModels;
using SpreadCheetah;
using SpreadCheetah.SourceGeneration;
using SpreadCheetah.Styling;

namespace CryptoWatcher.Infrastructure.Hyperliquid;

public class HyperliquidExcelService
{
    private const string TotalName = "Итого:";
    private const string EmptyValue = "-";
    private const string ReportSheetName = "Hyperliquid";

    private readonly IHyperliquidReportService _hyperliquidReportService;

    private static readonly Dictionary<string, Style> StyleNameToStyleMap = new()
    {
        [ExcelStyleRegistry.Number] =
            new Style { Format = NumberFormat.Standard(StandardNumberFormat.NoDecimalPlaces) },
        [ExcelStyleRegistry.TwoDecimalPlaces] = new Style
            { Format = NumberFormat.Standard(StandardNumberFormat.TwoDecimalPlaces) },
        [ExcelStyleRegistry.Percent] = new Style { Format = NumberFormat.Standard(StandardNumberFormat.Percent) },
    };

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

        foreach (var (styleName, style) in StyleNameToStyleMap)
        {
            sheet.AddStyle(style, styleName);
        }

        await sheet.StartWorksheetAsync(ReportSheetName,
            HyperliquidVaultPositionExcelContext.Default.HyperliquidVaultPositionExcelRow, ct);

        await sheet.AddHeaderRowAsync(HyperliquidVaultPositionExcelContext.Default.HyperliquidVaultPositionExcelRow,
            token: ct);

        foreach (var vaultReport in vaultReports)
        {
            foreach (var vaultReportItem in vaultReport.ReportItems)
            {
                var row = new HyperliquidVaultPositionExcelRow
                {
                    Vault = vaultReportItem.VaultAddress,
                    Balance = vaultReportItem.Balance,
                    Day = vaultReportItem.Day,
                    DailyProfit = vaultReportItem.DailyProfit,
                    DailyPercentProfit = vaultReportItem.DailyPercentProfit
                };

                await sheet.AddAsRowAsync(row,
                    HyperliquidVaultPositionExcelContext.Default.HyperliquidVaultPositionExcelRow, ct);
            }

            var totalRow = new HyperliquidVaultPositionExcelTotalRow
            {
                TotalName = TotalName,
                Day = EmptyValue,
                TotalBalance = vaultReport.TotalBalance,
                TotalAbsolutDailyProfit = vaultReport.TotalAbsoluteProfit,
                TotalDailyPercentProfit = vaultReport.TotalPercentProfit
            };

            await sheet.AddAsRowAsync(totalRow,
                HyperliquidVaultPositionExcelContext.Default.HyperliquidVaultPositionExcelTotalRow, ct);

            await sheet.AddRowAsync([], ct);
        }

        await sheet.FinishAsync(ct);
        ms.Seek(0, SeekOrigin.Begin);
        return ms;
    }
}

[WorksheetRow(typeof(HyperliquidVaultPositionExcelRow))]
[WorksheetRow(typeof(HyperliquidVaultPositionExcelTotalRow))]
internal partial class HyperliquidVaultPositionExcelContext : WorksheetRowContext;