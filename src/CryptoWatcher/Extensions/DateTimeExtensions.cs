namespace CryptoWatcher.Extensions;

public static class DateTimeExtensions
{
    public static DateOnly ToDateOnly(this DateTime dateTime)
    {
        return DateOnly.FromDateTime(dateTime);
    }

    public static DateTime ToDateTime(this DateOnly date)
    {
        return date.ToDateTime(TimeOnly.MinValue);
    }
}