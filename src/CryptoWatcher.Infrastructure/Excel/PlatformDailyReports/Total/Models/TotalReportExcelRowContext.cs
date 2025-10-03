using SpreadCheetah.SourceGeneration;

namespace CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Total.Models;

[WorksheetRow(typeof(TotalPlatformDailyReportExcelRow))]
[WorksheetRow(typeof(TotalPlatformDailyReportExcelTotalRow))]
internal partial class TotalReportExcelRowContext : WorksheetRowContext;