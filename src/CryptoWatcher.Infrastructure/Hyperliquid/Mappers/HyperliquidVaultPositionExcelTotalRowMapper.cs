using CryptoWatcher.HyperliquidModule.Models;
using CryptoWatcher.Infrastructure.Hyperliquid.ExcelModels;
using Riok.Mapperly.Abstractions;

namespace CryptoWatcher.Infrastructure.Hyperliquid.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class HyperliquidVaultPositionExcelTotalRowMapper
{
    public static partial HyperliquidVaultPositionExcelTotalRow MapToExcelModel(this HyperliquidDailyReport report,
        string totalName, string day);
}