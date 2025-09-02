using SpreadCheetah;
using SpreadCheetah.SourceGeneration;

namespace CryptoWatcher.Infrastructure.Excel;

internal class DateOnlyExcelConverter : CellValueConverter<DateOnly>
{
    public override DataCell ConvertToDataCell(DateOnly value)
    {
        return new DataCell(value.ToShortDateString());
    }
}