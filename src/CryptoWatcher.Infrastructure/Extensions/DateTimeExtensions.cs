namespace CryptoWatcher.Infrastructure.Extensions;

public static class DateTimeExtensions
{
    public static (DateOnly firstDay, DateOnly lastDay) GetCurrentMonthRange(this DateTime currentDate)
    {
        var firstDayOfCurrentMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
        var lastDayOfCurrentMonth = firstDayOfCurrentMonth.AddMonths(1).AddDays(-1);

        return (DateOnly.FromDateTime(firstDayOfCurrentMonth), DateOnly.FromDateTime(lastDayOfCurrentMonth));
    }
}