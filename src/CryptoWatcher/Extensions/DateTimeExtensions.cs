namespace CryptoWatcher.Extensions;

public static class DateTimeExtensions
{
    public static DateOnly ToDateOnly(this DateTime dateTime)
    {
        return DateOnly.FromDateTime(dateTime);
    }

    public static DateTime ToMinDateTime(this DateOnly date)
    {
        return date.ToDateTime(TimeOnly.MinValue);
    }
    
    public static DateTime ToMaxDateTime(this DateOnly date)
    {
        return date.ToDateTime(TimeOnly.MaxValue);
    }
}