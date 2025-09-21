using CryptoWatcher.Shared.Entities;

namespace CryptoWatcher.Abstractions.Reports;

public interface IDailySummaryReportProvider
{
    Task<Stream> CreateDailySummaryReportAsync(
        IReadOnlyCollection<Wallet> wallets,
        DateOnly from, DateOnly to, CancellationToken ct = default);
}