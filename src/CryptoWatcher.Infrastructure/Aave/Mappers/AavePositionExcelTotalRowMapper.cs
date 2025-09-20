using CryptoWatcher.AaveModule.Models;
using CryptoWatcher.Infrastructure.Aave.ExcelModels;
using Riok.Mapperly.Abstractions;

namespace CryptoWatcher.Infrastructure.Aave.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal static partial class AavePositionExcelTotalRowMapper
{
    public static partial AavePositionExcelTotalRow MapToExcelModel(this AaveDailyReport dailyReport, 
        string totalName);
}