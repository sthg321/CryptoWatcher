using CryptoWatcher.Modules.Hyperliquid.Models;
using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Hyperliquid.Models;
using Riok.Mapperly.Abstractions;

namespace CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Hyperliquid.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal static partial class HyperliquidVaultReportItemMapper
{
    public static partial HyperliquidVaultPositionExcelRow MapToExcelModel(this HyperliquidVaultReportItem item);
}