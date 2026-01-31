using System.Reflection;
 
using Hangfire.RecurringJobExtensions;

namespace CryptoWatcher.Infrastructure.Extensions;

public static class CronJobExtensions
{
    public static IEnumerable<Type> GetRecurringJobs(this Assembly assembly)
    {
        return assembly.ExportedTypes
            .Where(type => type.GetMethods().Any(info => info.GetCustomAttribute<RecurringJobAttribute>() is not null))
            .Select(type => type);
    }
}