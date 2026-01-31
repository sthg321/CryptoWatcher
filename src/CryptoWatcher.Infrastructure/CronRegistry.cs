namespace CryptoWatcher.Infrastructure;

internal static class CronRegistry
{
    public const string EveryMinute = "0 * * * *";
    public const string Every50Minutes = "0,50 * * * *";
}