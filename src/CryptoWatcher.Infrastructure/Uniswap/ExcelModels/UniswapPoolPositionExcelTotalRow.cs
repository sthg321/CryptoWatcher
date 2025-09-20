using CryptoWatcher.Infrastructure.Excel;
using CryptoWatcher.Shared.ValueObjects;
using SpreadCheetah.SourceGeneration;

namespace CryptoWatcher.Infrastructure.Uniswap.ExcelModels;

internal class UniswapPoolPositionExcelTotalRow
{
    public required string? TotalName { get; init; }  

    [CellStyle(ExcelStyleRegistry.TwoDecimalPlaces)]
    [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Money>))]
    public required Money PositionInUsd { get; init; }

    [CellStyle(ExcelStyleRegistry.TwoDecimalPlaces)]
    [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Money>))]
    public required Money TotalHoldInUsd { get; init; }

    [CellStyle(ExcelStyleRegistry.TwoDecimalPlaces)]
    [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Money>))]
    public required Money ProfitInUsd { get; init; }
    
    public required string TokenPairSymbols { get; init; } = null!;

    public required string Network { get; init; } = null!;
}