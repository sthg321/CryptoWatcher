using CryptoWatcher.Models;
using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Abstractions;

public interface IPlatformDailyReportDataProvider
{
    Task<PlatformDailyReportData> GetReportDataAsync(IReadOnlyCollection<Wallet> wallets,
        DateOnly from,
        DateOnly to,
        CancellationToken ct = default);
}