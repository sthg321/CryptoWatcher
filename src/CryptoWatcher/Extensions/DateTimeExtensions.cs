namespace CryptoWatcher.Extensions;

public static class DateTimeExtensions
{
    public static DateOnly ToDateOnly(this DateTime dateTime)
    {
        return DateOnly.FromDateTime(dateTime);
    }

    extension(DateOnly date)
    {
        public DateTimeOffset ToMinDateTime()
        {
            return new DateTimeOffset(date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc), TimeSpan.Zero);
        }

        public DateTimeOffset ToMaxDateTime()
        {
            return new DateTimeOffset(date.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc), TimeSpan.Zero);
        }
    }
}