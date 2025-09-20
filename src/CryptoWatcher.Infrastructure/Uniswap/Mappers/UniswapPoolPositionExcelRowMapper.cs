using CryptoWatcher.Infrastructure.Uniswap.ExcelModels;
using CryptoWatcher.UniswapModule.Models;
using Riok.Mapperly.Abstractions;

namespace CryptoWatcher.Infrastructure.Uniswap.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal static partial class UniswapPoolPositionExcelRowMapper
{
    public static partial UniswapPoolPositionExcelRow MapToExcelRowModel(this UniswapDailyReportItem item);
}