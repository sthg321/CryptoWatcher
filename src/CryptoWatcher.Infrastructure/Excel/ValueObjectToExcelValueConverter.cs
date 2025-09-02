using CryptoWatcher.Shared.ValueObjects;
using SpreadCheetah;
using SpreadCheetah.SourceGeneration;

namespace CryptoWatcher.Infrastructure.Excel;

public class ValueObjectToExcelValueConverter<TValueObject> : CellValueConverter<TValueObject>
{
    public override DataCell ConvertToDataCell(TValueObject value)
    {
        return value switch
        {
            Money money => new DataCell(money.Value),
            Percent percent => new DataCell(percent.Value),
            _ => throw new NotSupportedException()
        };
    }
}