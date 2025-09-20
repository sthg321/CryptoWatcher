using CryptoWatcher.Infrastructure.Excel;
using CryptoWatcher.Shared.ValueObjects;
using SpreadCheetah.SourceGeneration;

namespace CryptoWatcher.Infrastructure.Hyperliquid.ExcelModels;

public class HyperliquidVaultPositionExcelTotalRow
{
    /// <summary>
    /// empty value to save row order.
    /// </summary>
    [ColumnHeader("Адрес хранилища")]
    public string TotalName { get; init; } = null!;

    /// <summary>
    /// empty value to save row order.
    /// </summary>
    [ColumnHeader("День")] public string? Day { get; init; } = null;

    [ColumnHeader("Баланс")]
    [CellStyle(ExcelStyleRegistry.TwoDecimalPlaces)]
    [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Money>))]
    public Money PositionInUsd { get; init; }

    [ColumnHeader("Изменение за день")]
    [CellStyle(ExcelStyleRegistry.TwoDecimalPlaces)]
    [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Money>))]
    public Money ProfitInUsd { get; init; }

    [ColumnHeader("Изменение за день в процентах")]
    [CellStyle(ExcelStyleRegistry.Percent)]
    [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Percent>))]
    public Percent ProfitInPercent { get; init; }
}