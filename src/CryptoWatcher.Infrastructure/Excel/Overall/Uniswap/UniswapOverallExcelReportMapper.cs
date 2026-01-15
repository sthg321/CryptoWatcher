using CryptoWatcher.Modules.Uniswap.Application.Models.Reports;
using Riok.Mapperly.Abstractions;

namespace CryptoWatcher.Infrastructure.Excel.Overall.Uniswap;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class UniswapOverallExcelReportMapper
{
    public static partial UniswapOverallExcelReport Map(this UniswapOverallReport report);
}