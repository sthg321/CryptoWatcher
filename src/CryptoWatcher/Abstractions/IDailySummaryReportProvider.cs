using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Abstractions;

public interface IDailySummaryReportProvider
{
    Task<Stream> GetReportDataAsync(
        IReadOnlyCollection<Wallet> wallets,
        DateOnly from, DateOnly to, CancellationToken ct = default);
}