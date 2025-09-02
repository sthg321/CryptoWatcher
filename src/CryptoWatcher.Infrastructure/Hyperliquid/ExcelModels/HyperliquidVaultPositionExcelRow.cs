using CryptoWatcher.Infrastructure.Excel;
using CryptoWatcher.Shared.ValueObjects;
using SpreadCheetah.SourceGeneration;

namespace CryptoWatcher.Infrastructure.Hyperliquid.ExcelModels;

internal class HyperliquidVaultPositionExcelRow
{
    [ColumnHeader("Адрес хранилища")] public string Vault { get; init; } = null!;
    [ColumnHeader("День")] 
    [ColumnWidth(15)]
    [CellValueConverter(typeof(DateOnlyExcelConverter))]
    public DateOnly Day { get; init; }  

    [ColumnHeader("Баланс")]
    [ColumnWidth(20)]
    [CellStyle(ExcelStyleRegistry.TwoDecimalPlaces)]
    [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Money>))]
    public Money Balance { get; init; }

    [ColumnHeader("Изменение за день")]
    [ColumnWidth(20)]
    [CellStyle(ExcelStyleRegistry.TwoDecimalPlaces)]
    [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Money>))]
    public Money DailyProfit { get; init; }

    [ColumnHeader("Изменение за день в процентах")]
    [ColumnWidth(30)]
    [CellStyle(ExcelStyleRegistry.Percent)]
    [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Percent>))]
    public Percent DailyPercentProfit { get; init; }
}