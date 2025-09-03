using CryptoWatcher.Infrastructure.Uniswap.ExcelModels;
using CryptoWatcher.Infrastructure.Uniswap.Mappers;
using CryptoWatcher.UniswapModule.Services;
using SpreadCheetah;
using SpreadCheetah.SourceGeneration;

namespace CryptoWatcher.Infrastructure.Uniswap;

public interface IUniswapExcelReportService
{
    Task<Stream> ExportPoolInfoToExcelAsync(DateOnly? from, DateOnly? to, CancellationToken ct = default);
}

internal class UniswapExcelReportService : IUniswapExcelReportService
{
    private const string ReportName = "Uniswap";
    private const string TotalName = "Итого:";

    private readonly IUniswapReportService _uniswapReportService;

    public UniswapExcelReportService(IUniswapReportService uniswapReportService)
    {
        _uniswapReportService = uniswapReportService;
    }

    public async Task<Stream> ExportPoolInfoToExcelAsync(DateOnly? from, DateOnly? to, CancellationToken ct = default)
    {
        var now = DateTime.Now;
        var monthStart = new DateTime(now.Year, now.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        if (!from.HasValue || !to.HasValue)
        {
            from = DateOnly.FromDateTime(monthStart);
            to = DateOnly.FromDateTime(monthEnd);
        }

        var poolPositions = await _uniswapReportService.CreateReportAsync(from.Value, to.Value, ct);

        var ms = new MemoryStream();
        var sheet = await Spreadsheet.CreateNewAsync(ms, cancellationToken: ct);

        await sheet.StartWorksheetAsync(ReportName, PoolInfoExcelRowContext.Default.UniswapPoolPositionExcelRow,
            token: ct);

        await sheet.AddHeaderRowAsync(PoolInfoExcelRowContext.Default.UniswapPoolPositionExcelRow, token: ct);

        foreach (var poolPosition in poolPositions)
        {
            foreach (var positionSnapshot in poolPosition.ReportItems)
            {
                var excelRow = positionSnapshot.MapToExcelRowModel();

                await sheet.AddAsRowAsync(excelRow, PoolInfoExcelRowContext.Default.UniswapPoolPositionExcelRow, ct);
            }

            if (poolPosition.ReportItems.Count != 0)
            {
                // pool can contain data only for 1 network and 1 token pair, 
                // so we can take just the first item
                var item = poolPosition.ReportItems.First();
                var totalExcelRow = poolPosition.MapToExcelModel(TotalName, item.TokenPairSymbols, item.Network);

                await sheet.AddAsRowAsync(totalExcelRow,
                    PoolInfoExcelRowContext.Default.UniswapPoolPositionExcelTotalRow, ct);
            }

            await sheet.AddRowAsync([], ct);
        }

        await sheet.FinishAsync(ct);

        ms.Seek(0, SeekOrigin.Begin);

        return ms;
    }
}

[WorksheetRow(typeof(UniswapPoolPositionExcelRow))]
[WorksheetRow(typeof(UniswapPoolPositionExcelTotalRow))]
internal partial class PoolInfoExcelRowContext : WorksheetRowContext;