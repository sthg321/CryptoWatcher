using CryptoWatcher.Infrastructure.Uniswap.ExcelModels;
using CryptoWatcher.UniswapModule.Models;
using Riok.Mapperly.Abstractions;

namespace CryptoWatcher.Infrastructure.Uniswap.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal static partial class UniswapPoolPositionExcelTotalRowMapper
{
    public static partial UniswapPoolPositionExcelTotalRow MapToExcelModel(this UniswapDailyReport report,
        string totalName, string tokenPairSymbols, string network);
}