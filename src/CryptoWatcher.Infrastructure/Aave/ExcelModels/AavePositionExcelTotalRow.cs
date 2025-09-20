using CryptoWatcher.Infrastructure.Excel;
using CryptoWatcher.Shared.ValueObjects;
using SpreadCheetah.SourceGeneration;

namespace CryptoWatcher.Infrastructure.Aave.ExcelModels;

internal class AavePositionExcelTotalRow
{
    public required string TotalName { get; init; }

    public required string TokenSymbol { get; init; } = null!;
    
    [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Money>))]
    [CellStyle(ExcelStyleRegistry.TwoDecimalPlaces)]
    public required Money PositionInUsd { get; init; }
    
    [ColumnHeader("Рост позиции в $")]
    [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Money>))]
    [CellStyle(ExcelStyleRegistry.TwoDecimalPlaces)]
    public required Money PositionGrowInUsd { get; init; }
    
    [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Money>))]
    [CellStyle(ExcelStyleRegistry.TwoDecimalPlaces)]
    public required Money ProfitInUsd { get; init; }
    
    public required decimal PositionInToken { get; init; }
    
    public required decimal ProfitInToken { get; init; }
    
    // [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Money>))]
    // [CellStyle(ExcelStyleRegistry.TwoDecimal Places)]
    // public required Money ProfitInUsd { get; init; }

    // [ColumnHeader("Комиссия в $")]
    // [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Money>))]
    // [CellStyle(ExcelStyleRegistry.TwoDecimalPlaces)]
    // public required Money TotalCommissionInUsd { get; init; }
    //
    // [ColumnHeader("Комиссия в $/%")]
    // [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Percent>))]
    // [CellStyle(ExcelStyleRegistry.TwoDecimalPlaces)]
    // public required Percent TotalCommissionInUsdPercent { get; init; }
    //
    // [ColumnHeader("Комиссия в токена")]
    // public required decimal TotalCommissionInToken { get; init; }
    //
    // [ColumnHeader("Комиссия в $/%")]
    // [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Percent>))]
    // [CellStyle(ExcelStyleRegistry.Percent)]
    // public required Percent TotalCommissionInTokenPercent { get; init; }
}