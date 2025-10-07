using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Uniswap.Models;
using CryptoWatcher.Modules.Uniswap.Models;
using Riok.Mapperly.Abstractions;

namespace CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Uniswap.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal static partial class UniswapPoolPositionExcelTotalRowMapper
{
    public static partial UniswapPoolPositionExcelTotalRow MapToExcelModel(this UniswapDailyReport report,
        string totalName, string tokenPairSymbols, string network);
}