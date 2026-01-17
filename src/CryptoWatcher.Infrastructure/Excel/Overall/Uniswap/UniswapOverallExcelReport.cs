using CryptoWatcher.Shared.ValueObjects;
using SpreadCheetah.SourceGeneration;

namespace CryptoWatcher.Infrastructure.Excel.Overall.Uniswap;

public class UniswapOverallExcelReport
{
    [ColumnHeader("Сеть")]
    [ColumnWidth(20)]
    public string Network { get; init; } = null!;

    [ColumnHeader("Пара")]
    [ColumnWidth(25)]
    public string Pair { get; init; } = null!;

    [ColumnHeader("Дата открытия")]
    [ColumnWidth(25)]
    [CellStyle(ExcelStyleRegistry.Date)]
    [CellValueConverter(typeof(DateOnlyExcelConverter))]
    public DateOnly CreatedAt { get; init; }

    [ColumnHeader("Дата закрытия")]
    [ColumnWidth(25)]
    [CellStyle(ExcelStyleRegistry.Date)]
    [CellValueConverter(typeof(NullableDateOnlyExcelConverter))]
    public DateOnly? ClosedAt { get; init; }

    [ColumnHeader("Изначальный баланс $")]
    [ColumnWidth(40)]
    [CellStyle(ExcelStyleRegistry.Dollars)]
    [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Money>))]
    public Money InitialBalanceInUsd { get; init; }

    [ColumnHeader("Текущий баланс $")]
    [ColumnWidth(30)]
    [CellStyle(ExcelStyleRegistry.Dollars)]
    [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Money>))]
    public Money CurrentBalanceInUsd { get; init; }

    [ColumnHeader("Комиссия в $")]
    [ColumnWidth(30)]
    [CellStyle(ExcelStyleRegistry.Dollars)]
    [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Money>))]
    public Money CommissionInUsd { get; init; }

    [ColumnHeader("Награды в $")]
    [ColumnWidth(30)]
    [CellStyle(ExcelStyleRegistry.Dollars)]
    [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Money>))]
    public Money RewardsInUsd { get; init; }

    [ColumnHeader("PnL в $")]
    [ColumnWidth(30)]
    [CellStyle(ExcelStyleRegistry.Dollars)]
    [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Money>))]
    public Money Pnl { get; init; }

    [ColumnHeader("APR%")]
    [ColumnWidth(30)]
    [CellStyle(ExcelStyleRegistry.Percent)]
    [CellValueConverter(typeof(ValueObjectToExcelValueConverter<Percent>))]
    public Percent Apr { get; init; }
}