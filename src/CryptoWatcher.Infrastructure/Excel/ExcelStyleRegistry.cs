using SpreadCheetah;
using SpreadCheetah.Styling;

namespace CryptoWatcher.Infrastructure.Excel;

internal class ExcelStyleRegistry
{
    public const string Percent = "Percent";
    public const string TwoDecimalPlaces = "Decimal";
    public const string Number = "NoDecimalPlaces";

    public static void AddNamedStyles(Spreadsheet spreadsheet)
    {
        spreadsheet.AddStyle(new Style { Format = NumberFormat.Standard(StandardNumberFormat.NoDecimalPlaces) },
            Number);
        spreadsheet.AddStyle(new Style { Format = NumberFormat.Standard(StandardNumberFormat.Percent) },
            Percent);
        spreadsheet.AddStyle(new Style { Format = NumberFormat.Standard(StandardNumberFormat.TwoDecimalPlaces) },
            TwoDecimalPlaces);
    }
}