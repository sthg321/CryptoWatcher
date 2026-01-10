using CryptoWatcher.Shared.ValueObjects;
using SpreadCheetah.SourceGeneration;

namespace CryptoWatcher.Infrastructure.Excel.PlatformDailyReports.Uniswap.Models;

internal class UniswapPoolPositionExcelRow
{
    [ColumnHeader("День")]
    [CellValueConverter(typeof(DateOnlyExcelConverter))]
    public required DateOnly Day { get; init; }  

    [ColumnHeader("Позиция в $")]
    [ColumnWidth(20)]
    [CellStyle(ExcelStyleRegistry.TwoDecimalPlaces)]
    [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Money>))]
    public required Money PositionInUsd { get; init; }
    
    [ColumnHeader("HOLD $")]
    [ColumnWidth(20)]
    [CellStyle(ExcelStyleRegistry.TwoDecimalPlaces)]
    [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Money>))]
    public required Money HoldInUsd { get; init; }

    [ColumnHeader("Комиссия в $")]
    [ColumnWidth(20)]
    [CellStyle(ExcelStyleRegistry.TwoDecimalPlaces)]
    [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Money>))]
    public required Money DailyProfitInUsd { get; init; }
    
    [ColumnHeader("Награды в $")]
    [ColumnWidth(20)]
    [CellStyle(ExcelStyleRegistry.TwoDecimalPlaces)]
    [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Money>))]
    public required Money RewardsInUsd { get; init; }
 
    [ColumnHeader("Пара")] public required string TokenPairSymbols { get; init; } = null!;

    [ColumnHeader("Сеть")] public required string Network { get; init; } = null!;
}