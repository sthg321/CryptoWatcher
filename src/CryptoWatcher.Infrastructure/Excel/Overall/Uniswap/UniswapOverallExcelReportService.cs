using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports;
using CryptoWatcher.Models;
using CryptoWatcher.Modules.Uniswap.Application.Abstractions.Reports;
using CryptoWatcher.Shared.Entities;
using SpreadCheetah;

namespace CryptoWatcher.Infrastructure.Excel.Overall.Uniswap;

internal class UniswapOverallExcelReportService
{
    private readonly IExcelReportGenerator _excelReportGenerator;
    private readonly IUniswapOverallReportService _reportService;

    public UniswapOverallExcelReportService(IExcelReportGenerator excelReportGenerator,
        IUniswapOverallReportService reportService)
    {
        _excelReportGenerator = excelReportGenerator;
        _reportService = reportService;
    }

    public async Task<ExcelReport> CreateReportAsync(IReadOnlyCollection<Wallet> wallets, DateOnly from, DateOnly to,
        CancellationToken ct = default)
    {
        var reports = await _reportService.GetReportDataAsync(wallets, from, to, ct);

        var report = await _excelReportGenerator.CreateExcelWorkbookAsync(async spreadsheet =>
        {
            await spreadsheet.StartWorksheetAsync("uniswap_overall",
                UniswapOverallExcelRowContext.Default.UniswapOverallExcelReport, token: ct);

            await spreadsheet.AddHeaderRowAsync(UniswapOverallExcelRowContext.Default.UniswapOverallExcelReport, null, ct);
            foreach (var reportByWallet in reports)
            {
                await spreadsheet.AddRowAsync([new Cell("Кошелек:"), new Cell(reportByWallet.Key)], ct);
                await spreadsheet.AddRowAsync([], ct);

                foreach (var uniswapOverallReport in reportByWallet.Value)
                {
                    await spreadsheet.AddAsRowAsync(uniswapOverallReport.Map(),
                        UniswapOverallExcelRowContext.Default.UniswapOverallExcelReport, ct);
                }

                await spreadsheet.AddRowAsync([], ct);
            }
        }, ct);

        return new ExcelReport("uniswap_overall.xlsx", report);
    }
}