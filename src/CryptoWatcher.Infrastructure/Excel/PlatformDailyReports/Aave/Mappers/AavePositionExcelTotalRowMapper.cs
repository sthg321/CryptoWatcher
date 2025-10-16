using CryptoWatcher.Modules.Aave.Models;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Aave.Models;
using Riok.Mapperly.Abstractions;

namespace CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Aave.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal static partial class AavePositionExcelTotalRowMapper
{
    public static partial AavePositionExcelTotalRow MapToExcelModel(this AaveDailyReport dailyReport, 
        string totalName);
}