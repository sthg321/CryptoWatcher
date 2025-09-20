using CryptoWatcher.Infrastructure.Excel;
using CryptoWatcher.Shared.ValueObjects;
using SpreadCheetah.SourceGeneration;

namespace CryptoWatcher.Infrastructure.Aave.ExcelModels;

internal class AavePositionExcelRow
{
    [ColumnHeader("День")]
    [ColumnWidth(15)]
    [CellValueConverter(typeof(DateOnlyExcelConverter))]
    public required DateOnly Day { get; init; }

    [ColumnHeader("Токен")] 
    [ColumnWidth(10)]
    public required string TokenSymbol { get; init; } = null!;
    
    [ColumnHeader("Позиция в $")]
    [ColumnWidth(20)]
    [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Money>))]
    [CellStyle(ExcelStyleRegistry.TwoDecimalPlaces)]
    public required Money PositionInUsd { get; init; }
    
    [ColumnHeader("Позиция в токене")]
    [ColumnWidth(25)]
    public required decimal PositionInToken { get; init; }
    
    [ColumnHeader("Рост позиции в $")]
    [ColumnWidth(20)]
    [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Money>))]
    [CellStyle(ExcelStyleRegistry.TwoDecimalPlaces)]
    public required Money PositionGrowInUsd { get; init; }
    
    [ColumnHeader("Доход за день в $")]
    [ColumnWidth(20)]
    [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Money>))]
    [CellStyle(ExcelStyleRegistry.TwoDecimalPlaces)]
    public required Money DailyProfitInUsd { get; init; }
    
    [ColumnHeader("Доход за день в токене")]
    [ColumnWidth(25)]
    public required decimal DailyProfitInToken { get; init; }
    
    // [ColumnHeader("Динамика в $")]
    // [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Money>))]
    // [CellStyle(ExcelStyleRegistry.TwoDecimalPlaces)]
    // public required Money PositionChange { get; init; }
    //
    // [ColumnHeader("Комиссия в $")]
    // [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Money>))]
    // [CellStyle(ExcelStyleRegistry.TwoDecimalPlaces)]
    // public required Money CommissionInUsd { get; init; }
    //
    // [ColumnHeader("Комиссия в $/%")]
    // [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Percent>))]
    // [CellStyle(ExcelStyleRegistry.TwoDecimalPlaces)]
    // public required Percent CommissionInUsdPercent { get; init; }
    //
    // [ColumnHeader("Комиссия в токена")]
    // public required decimal CommissionInToken { get; init; }
    //
    // [ColumnHeader("Комиссия в $/%")]
    // [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Percent>))]
    // [CellStyle(ExcelStyleRegistry.Percent)]
    // public required Percent CommissionInTokenPercent { get; init; }
}