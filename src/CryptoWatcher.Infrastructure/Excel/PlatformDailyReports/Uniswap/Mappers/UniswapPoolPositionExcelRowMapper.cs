using CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Uniswap.Models;
using CryptoWatcher.Modules.Uniswap.Models;
using Riok.Mapperly.Abstractions;

namespace CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Uniswap.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal static partial class UniswapPoolPositionExcelRowMapper
{
    public static partial UniswapPoolPositionExcelRow MapToExcelRowModel(this UniswapDailyReportItem item);
}