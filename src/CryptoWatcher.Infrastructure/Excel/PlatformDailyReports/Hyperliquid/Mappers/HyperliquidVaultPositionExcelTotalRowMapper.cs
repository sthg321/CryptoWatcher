using CryptoWatcher.Modules.Hyperliquid.Models;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Hyperliquid.Models;
using Riok.Mapperly.Abstractions;

namespace CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Hyperliquid.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class HyperliquidVaultPositionExcelTotalRowMapper
{
    public static partial HyperliquidVaultPositionExcelTotalRow MapToExcelModel(this HyperliquidDailyReport report,
        string totalName, string day);
}